var anOpen = [];
var oTable;


$(document).ready(function () {
    var objSelectFiles = FormatFileSelectColumnJSON(objFiles);


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
        //"bProcessing": true,
        //"bServerSide": true,
        "bFilter": true
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


    $.post(sCurrentFilesURL, {})
        .done(function (data) {
            CurrentFiles = data;
        });
    $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous

    return CurrentFiles;
}
var objFiles = CurrentFiles();





