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

    $.getJSON(sCurrentFilesURL, {}, function (data) {
        CurrentFiles = data;
    });
    $.ajaxSetup({ async: true }); //Sets ajax back up to synchronous

    return CurrentFiles;
}
var objFiles = CurrentFiles();

$(document).ready(function () {
    var objSelectFiles = FormatFileSelectColumnJSON(objFiles);

    objSelectFiles.forEach(function (obj) {
        $("#FileList").append("<option value=\"" + obj.value + "\">" + obj.label + "</option>");
    });
    $("#FileList").multiselect({
        multiple: false,
        //hide: "explode",
        selectedList: 1 //this is what puts the selected value onto the select box...
    });

    $("#Generate")
        .button()
        .click(function (event) {
            objTemp = $('#FileList');

            var input = $("<input>")
               .attr("type", "hidden")
               .attr("name", "filename").val(String($('#FileList').val()));

            $('#frmGenerateData').append(input);
            $("#frmGenerateData").submit();
     });
});



