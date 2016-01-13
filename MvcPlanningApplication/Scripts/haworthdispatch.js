var oTable;
var anOpen = [];


$(document).ready(function () {
    var oTimerId;


    $("#ShipByDateFromFilter").datepicker();
    $("#ShipByDateToFilter").datepicker();
    $("#DockDateFromFilter").datepicker();
    $("#DockDateToFilter").datepicker();

    oTable = $('#objItems').DataTable({
        "bJQueryUI": true,
        "bProcessing": true,
        "bServerSide": true,
        "bFilter": true,
        "sAjaxSource": sGetDataUrl,// document.URL,
        "sServerMethod": "POST",
        "fnServerData": function (sSource, aoData, fnCallback, oSettings) {
            window.clearTimeout(oTimerId); //clear the timer if it still exists
            oTimerId = window.setTimeout(function () {
                var strTemp;


                //aoData.push({ "name": "MaxRecordCount", "value": intMaxRecordCount }); //adds the MaxRecordCount data to the array sent to the server...
                AppendAdditionalParameters(aoData)

                oSettings.jqXHR = $.ajax({  //This is the setting that does the posting of the data...
                    "dataType": 'json',
                    "type": "POST",
                    "url": sSource,
                    "data": aoData,
                    "success": fnCallback
                });
            }, 1000); //wait 1000 milliseconds (1 sec) before executing the function...
        },
        "aoColumns": [
            {
                "mDataProp": null, //Note that I had a problem with this column being first because when the datatable loads, it automatically sorts based on the first column; since this column had a null value
                "sWidth": 60,
                "sClass": "printIDLabel center", //applies the control class to the cell and the center class(which center aligns the image)
                "bSortable": false,
                "sDefaultContent": '<img src="' + sOpenImageUrl + '">'
            },
            { "mDataProp": "JobOrder" },
            { "mDataProp": "CustomerOrder" },
            { "mDataProp": "PurchaseOrder" },
            { "mDataProp": "SalesOrder" },
            { "mDataProp": "QuantityOrdered" },
            { "mDataProp": "ItemNumber" },
            { "mDataProp": "Shell" },
            { "mDataProp": "Frame" },
            { "mDataProp": "Fabric" },
            { "mDataProp": "ArmCaps" },
            {
                "mDataProp": "ShipByDate",
                "render": function (data, type, full, meta) {
                    return FormatDate(data);
                }
            },
            {
                "mDataProp": "DockDate",
                "render": function (data, type, full, meta) {
                    return FormatDate(data);
                }
            }
        ]
    });

    $('#objItems tbody').on('click', 'td.printIDLabel', function () {
        var nTr;
        var rowIndex;
        var objRecord;


        nTr = this.parentNode;
        i = $.inArray(nTr, anOpen);
        
        rowIndex = oTable.row(nTr).index(); //get the index of the current row

        var tr = $(this).closest('tr'); //tr = this.parentNode;
        var row = oTable.row(tr);
        if (row.child.isShown()) {
            $('img', this).attr('src', sOpenImageUrl);// This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            $('img', this).attr('src', sCloseImageUrl);// Open this row

            objRecord = oTable.row(rowIndex).data();
            row.child('<embed id=\"IDLabel\"  src=\"' + sGetPrintIDURL +
                "?&CustomerOrder='" + objRecord.CustomerOrder + "'" +
                "&PurchaseOrder='" + objRecord.PurchaseOrder + "'" +
                "&SalesOrder='" + objRecord.SalesOrder + "'" + '\" />', 'details').show();
            tr.addClass('shown');
        }
    });
});


function AppendAdditionalParameters(aoData) {
    var strTemp;


    /*I add the FixedColumnHeaders list to the data array that is sent to the server. This is a custom implementation to accomodate the fact that when datatables implements reorderable columns, the sSearch
    * variables get sent to the server based on the old column position whereas mDataProp gets sent based on the new column position. I am then unable to implement a multicolumn search because the data property and
    * search property don't align. So I send this FixedColumnHeaders to the server for use in searching based on the corresponding Ssearch variables. This list must match exactly the columns in DataTables table instantiation. 
    * I had previously implemented this in the server side code, but then any time my UI changed I would need to recompile the web service... So I fixed the implementation...*/
    aoData.push({
        "name": "FixedColumnHeaders",
        "value": [null, "JobOrder", "CustomerOrder", "PurchaseOrder", "SalesOrder", "QuantityOrdered", "ItemNumber", "Shell", "Frame", "Fabric", "ArmCaps", "ShipByDate", "DockDate", "StatusCode", "RequiredQty", "DockDate"]
    });

    /*iterates through the array and updates the appropriate object using the below 'case' statements. I was having an issue where sSearch was getting populated twice (ie sSearch_4 & sSearch_7 would contain the same search string) 
    * I solved the issue by manually setting sSearch in my aoData array below. Note that I populate the corresponding bRegex variable; this value isn't used anywhere but could be for future implementations...*/
    for (var i = 0; i < aoData.length; i++) {
        switch (aoData[i].name) {
            case "sSearch_1":
                aoData[i].value = $('#JobNumberFilter').val();
                break;
            case "bRegex_1":
                aoData[i].value = true;
                break;
            case "sSearch_2":
                aoData[i].value = $('#CONumberFilter').val();
                break;
            case "bRegex_2":
                aoData[i].value = true;
                break;
            case "sSearch_3":
                aoData[i].value = $('#PONumberFilter').val();
                break;
            case "bRegex_3":
                aoData[i].value = true;
                break;
            case "sSearch_4":
                aoData[i].value = $('#SONumberFilter').val();
                break;
            case "bRegex_4":
                aoData[i].value = true;
                break;
            case "sSearch_6":
                aoData[i].value = $('#ItemNumberFilter').val();
                break;
            case "bRegex_6":
                aoData[i].value = true;
                break;
            case "sSearch_7":
                aoData[i].value = $('#ShellFilter').val();
                break;
            case "bRegex_7":
                aoData[i].value = true;
                break;
            case "sSearch_8":
                aoData[i].value = $('#FrameFilter').val();
                break;
            case "bRegex_8":
                aoData[i].value = true;
                break;
            case "sSearch_9":
                aoData[i].value = $('#FabricFilter').val();
                break;
            case "bRegex_9":
                aoData[i].value = true;
                break;
            case "sSearch_10":
                aoData[i].value = $('#ArmCapsFilter').val();
                break;
            case "bRegex_10":
                aoData[i].value = true;
                break;
            case "sSearch_11":
                aoData[i].value = $('#ShipByDateFromFilter').val() + '~' +
                        $('#ShipByDateToFilter').val();
                break;
            case "bRegex_11":
                aoData[i].value = true;
                break;
            case "sSearch_12":
                aoData[i].value = $('#DockDateFromFilter').val() + '~' +
                        $('#DockDateToFilter').val();
                break;
            case "bRegex_12":
                aoData[i].value = true;
                break;
        }
    }
}