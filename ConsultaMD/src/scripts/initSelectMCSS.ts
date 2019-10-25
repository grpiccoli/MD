$(document).ready(function () {
    var elems = document.querySelectorAll('select');
    var instances = M.FormSelect.init(elems, {});
    //Fix iOS
    $('input.select-dropdown').on('touchend', function (e) { $(e.target).next('select').focus(); });
});