var oTable;


$(document).ready(function () {
    var oTimerId;


    $("#btnDownload").button()
        .click(function (event) {
            //event.preventDefault();
            var blah = oTable.ajax;

            var params = oTable.ajax.params();
            //var aoData = $.makeArray(params)
            //var additionalParams = [];
            var aoData = [];

            $.each(params, function (index, value) {
                aoData.push({
                    "name": index,
                    "value": value
                });
            });

            AppendAdditionalParameters(aoData);

            
            //aoData = $.extend(aoData, params)
            //aoData = $.extend(aoData, additionalParams)
            /*$.each(additionalParams, function (index, value) {
                var object = {};
                var name = value.name;
                object[name] = value.value;
                aoData = $.extend(params, [value.name, value.value])
            });*/
            
            var iframe = document.createElement('iframe');
            iframe.style.height = "0px";
            iframe.style.width = "0px";
            iframe.src = sGetDataUrl + "?" + $.param(aoData) + "&Format=Excel" //parameterizes the json array aoData and appends it to the URL; HTTP GET standard only allows parameters to be sent via the URL
            document.body.appendChild(iframe);
        });

    $("#GenerateDispatch")
        .button()
        .click(function (event) {

            var statesdemo = {
                state0: {
                    title: 'Generate Dispatch List...',
                    buttons: { Cancel: 0, 'Generate': 1 },
                    focus: 1, //sets the focus on to the Generate button...
                    submit: function (e, v, m, f) {
                        if (v == 0) { }
                        else if (v == 1) {
                            $.ajaxSetup({ async: false, dataType: "json" });
                            $.post(sGenerateDispatchDataUrl, null, function (data) {
                                if (data.Success) {
                                    $.prompt(data.Message);
                                    oTable.draw();
                                }
                                else {
                                    $.prompt(data.Message);
                                }
                            });
                            $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous
                        }
                    }
                }
            };
            $.prompt(statesdemo);



            
        });

    $("#RemainingOrdersOnly").button();
    $("#RemainingOrdersOnlyLabel").text("Show All Orders");
    $("#RemainingOrdersOnly").prop('checked', false);
    $("#RemainingOrdersOnly").change(function () {
        if (this.checked) {
            $("#RemainingOrdersOnlyLabel").text("Show Only Remaining Orders");
        }
        else
        {
            $("#RemainingOrdersOnlyLabel").text("Show All Orders");
        }
    });

    $("#UseLiveData").button();
    $("#UseLiveDataLabel").text("Use Live Data");
    $("#UseLiveData").prop('checked', false);
    $("#UseLiveData").change(function () {
        if (this.checked) {
            $("#UseLiveDataLabel").text("Use Static Data");
        }
        else {
            $("#UseLiveDataLabel").text("Use Live Data");
        }
    });

    $("#ShipByDateFromFilter").datepicker();
    $("#ShipByDateToFilter").datepicker();
    $("#DockDateFromFilter").datepicker();
    $("#DockDateToFilter").datepicker();

    oTable = $('#objItems').DataTable({
        "bJQueryUI": true,
        "bProcessing": true,
        "bServerSide": true,
        "bFilter": true,
        "sDom": 'T<"clear">RlrtBip',
        buttons: [
            'copyHtml5',
            'excelHtml5',
            'csvHtml5',
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'Legal'
            }
        ],
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
                "mDataProp": "JobOrder", //Note that I had a problem with this column being first because when the datatable loads, it automatically sorts based on the first column; since this column had a null value
                "sWidth": 60,
                "sClass": "printIDLabel center", //applies the control class to the cell and the center class(which center aligns the image)
                "bSortable": false,
                "render": function (data, type, full, meta) {
                    return '<img src="' + sOpenImageUrl + '">'
                }
            },
            { "mDataProp": "JobOrder" },
            { "mDataProp": "CustomerOrder" },
            { "mDataProp": "PurchaseOrder" },
            { "mDataProp": "SalesOrder" },
            { "mDataProp": "QuantityOrdered" },
            { "mDataProp": "QuantityRemaining" },
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

    //This is so that you can cause a search to occur on keyup for input field by adding class="dtSearchField" to it's html...
    $('input.dtSearchField').on('keyup change', function () {
        oTable.draw(); //forces the table to redraw and the search criteria is set above
    });

    $('#objItems tbody').on('click', 'td.printIDLabel', function () {
        var nTr;
        var rowIndex;
        var objRecord;
        var sHTML;

        nTr = this.parentNode;
        
        rowIndex = oTable.row(nTr).index(); //get the index of the current row
        objRecord = oTable.row(rowIndex).data();

        var ua = window.navigator.userAgent;
        var msie = ua.indexOf("MSIE ");
        if (msie > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./))//the browser is IE...
        {
            sHTML = '<object type=\"application/pdf\" id=\"IDLabel\"  data=\"' + sGetPrintIDURL +
                "?&CustomerOrder=" + objRecord.CustomerOrder +
                "&PurchaseOrder=" + objRecord.PurchaseOrder +
                "&SalesOrder=" + objRecord.SalesOrder +
                "&JobOrder=" + objRecord.JobOrder + '\" />';
        }
        else
        {
            sHTML = '<embed id=\"IDLabel\"  src=\"' + sGetPrintIDURL +
                "?&CustomerOrder=" + objRecord.CustomerOrder +
                "&PurchaseOrder=" + objRecord.PurchaseOrder +
                "&SalesOrder=" + objRecord.SalesOrder +
                "&JobOrder=" + objRecord.JobOrder + '\" />';
        }
        
        document.getElementById('embedHolder').innerHTML = sHTML;
        // Open this Datatable as a modal dialog box.
        $('#embedHolder').dialog({
            modal: false,
            resizable: true,
            position: { my: 'center + center', at: 'center + top', of: $(this).closest('tr') },
            width: 450,
            height: 275,
            autoResize: true,
            title: 'Job ' + objRecord.JobOrder
        });
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
        "value": [0, "JobOrder", "CustomerOrder", "PurchaseOrder", "SalesOrder", "QuantityOrdered", "QuantityRemaining", "ItemNumber", "Shell", "Frame", "Fabric", "ArmCaps", "ShipByDate", "DockDate"]
    });
    aoData.push({
        "name": "RemainingOrdersOnly",
        "value": $("#RemainingOrdersOnly").is(':checked')
    });
    aoData.push({
        "name": "UseLiveData",
        "value": $("#UseLiveData").is(':checked')
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
            case "sSearch_7":
                aoData[i].value = $('#ItemNumberFilter').val();
                break;
            case "bRegex_7":
                aoData[i].value = true;
                break;
            case "sSearch_8":
                aoData[i].value = $('#ShellFilter').val();
                break;
            case "bRegex_8":
                aoData[i].value = true;
                break;
            case "sSearch_9":
                aoData[i].value = $('#FrameFilter').val();
                break;
            case "bRegex_9":
                aoData[i].value = true;
                break;
            case "sSearch_10":
                aoData[i].value = $('#FabricFilter').val();
                break;
            case "bRegex_10":
                aoData[i].value = true;
                break;
            case "sSearch_11":
                aoData[i].value = $('#ArmCapsFilter').val();
                break;
            case "bRegex_11":
                aoData[i].value = true;
                break;
            case "sSearch_12":
                aoData[i].value = $('#ShipByDateFromFilter').val() + '~' +
                        $('#ShipByDateToFilter').val();
                break;
            case "bRegex_12":
                aoData[i].value = true;
                break;
            case "sSearch_13":
                aoData[i].value = $('#DockDateFromFilter').val() + '~' +
                        $('#DockDateToFilter').val();
                break;
            case "bRegex_13":
                aoData[i].value = true;
                break;
        }
    }
}