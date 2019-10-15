$(document).ready(function () {
    $('select').formSelect();
    //Fix iOS
    $('input.select-dropdown').on('touchend', function (e) { $(e.target).next('select').focus(); });
});