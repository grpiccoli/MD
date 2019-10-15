$(document).ready(function () {
    $('select').formSelect();
    $('input.select-dropdown').on('touchend', function (e) { $(e.target).next('select').focus(); });
});
//# sourceMappingURL=initSelectMCSS.js.map