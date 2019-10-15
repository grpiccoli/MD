document.addEventListener('DOMContentLoaded', function () {
    var elems = document.querySelectorAll('select');
    var instances = M.FormSelect.init(elems);
});
var modal_elems = $('#prevision-modal');
var prev_modal = M.Modal.init(modal_elems);
$('#prevision').change(function () {
    var prevision = $(this).val();
    if (prevision) {
        if (prevision > 2)
            $("#prev-pass-field").slideDown();
        else
            $("#prev-pass-field").slideUp();
    }
});
//# sourceMappingURL=signup.js.map