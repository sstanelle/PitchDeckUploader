$(function () {
    var dropZoneId = "drop-zone";
    var mouseOverClass = "mouse-over";
    var dropZone = $("#" + dropZoneId);

    document.getElementById(dropZoneId).addEventListener("dragover", function (e) {
        e.preventDefault();
        e.stopPropagation();

        dropZone.addClass(mouseOverClass);
    }, true);

    document.getElementById(dropZoneId).addEventListener("drop", function (e) {
        e.preventDefault();
        e.stopPropagation();

        $("#" + dropZoneId).removeClass(mouseOverClass);

        var files = e.dataTransfer.files;
        $('#uploadedFile').prop('files', files);
    }, true);

    document.getElementById(dropZoneId).addEventListener("dragleave", function (e) {
        $("#" + dropZoneId).removeClass(mouseOverClass);
    }, true);

})