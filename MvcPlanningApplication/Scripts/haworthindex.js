var oTable;
var blnCheckChanged;


$(document).ready(function () {
    var objSelectFiles = FormatFileSelectColumnJSON(objFiles);
    var oTimerId;


    $("#ChangeDateFromFilter").datepicker();
    $("#ChangeDateToFilter").datepicker();
    $("#DockDateFromFilter").datepicker();
    $("#DockDateToFilter").datepicker();
    $("#ImportDateTimeFromFilter").datepicker();
    $("#ImportDateTimeToFilter").datepicker();

    objStatusCodes.forEach(function (obj) {
        $("#StatusCodeFilter").append("<option value=\"" + obj + "\">" + obj + "</option>");
    });
    $("#StatusCodeFilter").multiselect({
        multiple: true,
        hide: "explode",
        minWidth: 300,
        //header: false, // "Select a Work Order Type",
        noneSelectedText: "Select Type",
        //selectedList: 1 //this is what puts the selected value onto the select box...
    });

    objCharacteristics.forEach(function (obj) {
        $("#CharacteristicsFilter").append("<option value=\"" + obj + "\">" + obj + "</option>");
    });
    $("#CharacteristicsFilter").multiselect({
        multiple: true,
        hide: "explode",
        minWidth: 300,
        //header: false, // "Select a Work Order Type",
        noneSelectedText: "Select Type"
        //selectedList: 1 //this is what puts the selected value onto the select box...
    });

    objSelectFiles.forEach(function (obj) {
        $("#FileList").append("<option value=\"" + obj.value + "\">" + obj.label + "</option>");
    });
    $("#FileList").multiselect({
        multiple: false,
        //hide: "explode",
        selectedList: 1, //this is what puts the selected value onto the select box...
        click: function (e) {
            //alert("Dood");
        }
    });

    /*I wanted to avoid a query in the situation where a user just opens the select box to observe the checked values. So I created a global variable that gets set only when a user
     * checks/unchecks an option or CheckAll or UnCheckAll.*/
    $("select").multiselect({ //makes all the selects multiselect boxes AND applies the specified methods...
        //hide: "explode",
        checkAll: function () {
            blnCheckChanged = true;
        },
        uncheckAll: function () {
            blnCheckChanged = true;
        },
        click: function (event, ui) {
            blnCheckChanged = true;
        },
        close: function () {
            //causes a search if any values have been checked...
            if (blnCheckChanged) {
                blnCheckChanged = false;
                oTable.draw();
            }
        }
    });

    $("#Generate")
        .button()
        .click(function (event) {
            function ExcelRanges(strFile) {
                $.ajaxSetup({ async: false, dataType: "json" });

                $.post(sGetExcelRanges, { File: strFile })
                    .done(function (data) {
                        ExcelRanges = data;
                    });
                $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous

                return ExcelRanges;
            }
            var objExcelRanges = ExcelRanges($('#FileList').val());


            var statesdemo = {
                state0: {
                    title: 'Select A Range...',
                    html: '<form class="HaworthDataGenerator" id="HaworthDataGeneratorStep1"><div class="error"></div>' +
                        '<br><select title="Select Excel Range" id="ExcelRanges" required needsSelection></select>' +
                        '</form>',
                    buttons: { Cancel: 0, 'Generate': 1 },
                    focus: 1, //sets the focus on to the Generate button...
                    submit: function (e, v, m, f) {
                        if (v == 0) { }
                        else if (v == 1) {
                            $('#HaworthDataGeneratorStep1').validate({
                                ignore: ':hidden:not("#ExcelRanges")', //Tells it to ignore hidden fields except for the one with id ExcelRangeName
                                errorLabelContainer: $("#WorkOrderWizardStep2 div.error")
                            });

                            if ($("#HaworthDataGeneratorStep1").children("select").valid()) {
                                var objData = {
                                    SelectedFile: $('#FileList').val(),
                                    SelectedRange: $('#ExcelRanges').val()
                                }

                                $.post(sGenerateDataURL, objData, function (data) {
                                    if (data.Success) {
                                        $.prompt(data.Message);
                                        oTable.draw();
                                    }
                                    else {
                                        $.prompt(data.Message);
                                    }
                                });

                            }
                            else {
                                e.preventDefault();
                            }
                        }
                    }
                }
            };
            $.prompt(statesdemo);

            objExcelRanges.forEach(function (obj) {
                $("#ExcelRanges").append("<option value=\"" + obj + "\">" + obj + "</option>");
            });

            $("#ExcelRanges").multiselect({
                multiple: false,
                selectedList: 1
            });

        });

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
                //"render": makeGetCharacteristicsBtn,
                "mDataProp": "ID", //Note that I had a problem with this column being first because when the datatable loads, it automatically sorts based on the first column; since this column had a null value
                "sWidth": 60,
                "sClass": "control characteristics center", //applies the control class to the cell and the center class(which center aligns the image)
                "bSortable": false,
                "render": function (data, type, full, meta) {
                    return '<img src="' + sOpenImageUrl + '">'
                }
            },
            {
                "mDataProp": "ChangeDate",
                "render": function (data, type, full, meta) {
                    var objDate = new Date(data);
                    return dateFormat(objDate, "mm-dd-yyyy");  //could also use "mm-dd-yyyy HH:MMTT" which returns 11-01-2012 12:30:00...
                }
            },
            { "mDataProp": "OrderNumber" },
            { "mDataProp": "ItemNumber" },
            { "mDataProp": "PartInformation.Description" },
            { "mDataProp": "PartInformation.Description2" },
            { "mDataProp": "PartInformation.ColorCode" },
            { "mDataProp": "PartInformation.ColorPattern" },
            { "mDataProp": "PartInformation.ColorDescription" },
            { "mDataProp": "StatusCode" },
            { "mDataProp": "RequiredQty" },
            {
                "mDataProp": "DockDate",
                "render": function (data, type, full, meta) {
                    var objDate = new Date(data);
                    return dateFormat(objDate, "mm-dd-yyyy");
                }
            },
            {
                "mDataProp": "ImportDateTime",
                "render": function (data, type, full, meta) {
                    var objDate = new Date(data);
                    return dateFormat(objDate, "mm-dd-yyyy");
                }
            },
            {
                "mDataProp": "DeliveryInformation",
                "render": function (data, type, full, meta) {
                    return data.Address + '<br />' +
                        (data.Address2 && data.Address2 + '<br />') +
                        data.City + ', ' + data.State + ' ' + data.ZipCode
                }
            },
            { "mDataProp": "UnitPrice" }
        ]
    });

    //This is so that you can cause a search to occur on keyup for input field by adding class="dtSearchField" to it's html...
    $('input.dtSearchField').on('keyup change', function () {
        oTable.draw(); //forces the table to redraw and the search criteria is set above
    });

    $('#objItems tbody').on('click', 'td.control', function () {
        var nTr;
        var rowIndex;
        var objRecord;

        nTr = this.parentNode;

        rowIndex = oTable.row(nTr).index(); //get the index of the current row

        //alert(oTable.fnGetData(oTable.$('tr.row_selected')[0])[rowIndex].co_num);// works!!
        objRecord = oTable.row(rowIndex).data();

        $("#CharacteristicsDialogDiv").html(GetCharacteristicsTableHTML());

        oCharacteristicsTable = $('#objCharacteristics').DataTable({
            "bJQueryUI": true,
            "aaData": objRecord.Characteristics["$values"],
            "sDom": "Rlfrtip", //Enables column reorder with resize
            "sDom": '<"top">rt<"bottom"flp><"clear">', //hides footer that displays 'showing record 1 of 1'...
            "bProcessing": true,
            "bFilter": false,   //hides the search box
            "bPaginate": false, //disables paging functionality
            "aoColumns": [
                { "mDataProp": "Characteristic" },
                { "mDataProp": "Value" }]
        });

        // Open this Datatable as a modal dialog box.
        $('#CharacteristicsDialogDiv').dialog({
            modal: false,
            resizable: true,
            position: { my: 'center + center', at: 'center + top', of: $(this).closest('tr') },
            width: 'auto',
            autoResize: true,
            title: ' Order ' + objRecord.OrderNumber
        });
    });
});

function FormatFileSelectColumnJSON(x) {
    var finalEdit = new Array();

    //Loop through the list
    for (i = 0; i < x.files.length; i++) {
        //because I use getJSON when doing a GET in my Ajax call, I have strongly typed JSON objects to use below...
        //finalEdit[i] = { value: x[i].WOTYPE, label: x[i].DESCRIPTION }; //when I switched to using LinqToCSV, tables with a column that had the same name, had _column appended to them...
        finalEdit[i] = { value: x.files[i].FullPath, label: x.files[i].name };
    }

    return finalEdit;
}

function CurrentFiles() {
    $.ajaxSetup({ async: false, dataType: "json" });


    $.post(sGetCurrentFilesURL, {})
        .done(function (data) {
            CurrentFiles = data;
        });
    $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous

    return CurrentFiles;
}
var objFiles = CurrentFiles();

function StatusCodes() {
    $.ajaxSetup({ async: false, dataType: "json" });


    $.post(sGetStatusCodesURL, {})
        .done(function (data) {
            StatusCodes = data;
        });
    $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous

    return StatusCodes;
}
var objStatusCodes = StatusCodes();

function Characteristics() {
    $.ajaxSetup({ async: false, dataType: "json" });


    $.post(sGetCharacteristicsURL, {})
        .done(function (data) {
            Characteristics = data;
        });
    $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous

    return Characteristics;
}
var objCharacteristics = Characteristics();

function AppendAdditionalParameters(aoData) {
    var strTemp;


    /*I add the FixedColumnHeaders list to the data array that is sent to the server. This is a custom implementation to accomodate the fact that when datatables implements reorderable columns, the sSearch
    * variables get sent to the server based on the old column position whereas mDataProp gets sent based on the new column position. I am then unable to implement a multicolumn search because the data property and
    * search property don't align. So I send this FixedColumnHeaders to the server for use in searching based on the corresponding Ssearch variables. This list must match exactly the columns in DataTables table instantiation. 
    * I had previously implemented this in the server side code, but then any time my UI changed I would need to recompile the web service... So I fixed the implementation...*/
    aoData.push({
        "name": "FixedColumnHeaders",
        "value": ["Characteristics", "ChangeDate", "OrderNumber", "ItemNumber", "Description", "Description2", "ColorCode", "ColorPattern", "ColorDescription", "StatusCode", "RequiredQty", "DockDate", "ImportDateTime",
            "PlantAddress", "UnitPrice"]
    });

    /*iterates through the array and updates the appropriate object using the below 'case' statements. I was having an issue where sSearch was getting populated twice (ie sSearch_4 & sSearch_7 would contain the same search string) 
    * I solved the issue by manually setting sSearch in my aoData array below. Note that I populate the corresponding bRegex variable; this value isn't used anywhere but could be for future implementations...*/
    for (var i = 0; i < aoData.length; i++) {
        switch (aoData[i].name) {
            case "sSearch_0":
                strTemp = String($('#CharacteristicsFilter').val());
                if (strTemp !== 'null')
                    aoData[i].value = strTemp.split(',').join('|');
                else
                    aoData[i].value = '';
                break;
            case "bRegex_0": 0
                aoData[i].value = true;
                break;
            case "sSearch_1":
                aoData[i].value = $('#ChangeDateFromFilter').val() + '~' +
                        $('#ChangeDateToFilter').val();
                break;
            case "bRegex_1":
                aoData[i].value = true;
                break;
            case "sSearch_2":
                aoData[i].value = $('#OrderNumberFilter').val();
                break;
            case "bRegex_2":
                aoData[i].value = true;
                break;
            case "sSearch_3":
                aoData[i].value = $('#ItemNumberFilter').val();
                break;
            case "bRegex_3":
                aoData[i].value = true;
                break;
            case "sSearch_4":
                aoData[i].value = $('#DescriptionFilter').val();
                break;
            case "bRegex_4":
                aoData[i].value = true;
                break;
            case "sSearch_5":
                aoData[i].value = $('#Description2Filter').val();
                break;
            case "bRegex_5":
                aoData[i].value = true;
                break;
            case "sSearch_6":
                aoData[i].value = $('#ColorCodeFilter').val();
                break;
            case "bRegex_6":
                aoData[i].value = true;
                break;
            case "sSearch_7":
                aoData[i].value = $('#ColorPatternFilter').val();
                break;
            case "bRegex_7":
                aoData[i].value = true;
                break;
            case "sSearch_8":
                aoData[i].value = $('#ColorDescriptionFilter').val();
                break;
            case "bRegex_8":
                aoData[i].value = true;
                break;
            case "sSearch_9":
                strTemp = String($('#StatusCodeFilter').val());
                if (strTemp !== 'null')
                    aoData[i].value = strTemp.split(',').join('|');
                else
                    aoData[i].value = '';
                break;
            case "bRegex_9":
                aoData[i].value = true;
                break;
            case "sSearch_11":
                aoData[i].value = $('#DockDateFromFilter').val() + '~' +
                        $('#DockDateToFilter').val();
                break;
            case "bRegex_11":
                aoData[i].value = true;
                break;
            case "sSearch_12":
                aoData[i].value = $('#ImportDateTimeFromFilter').val() + '~' +
                        $('#ImportDateTimeToFilter').val();
                break;
            case "bRegex_12":
                aoData[i].value = true;
                break;
        }
    }
}