document.addEventListener('DOMContentLoaded', function (_) {
    var rutId = "Input_RUT";
    var $rut = $("#" + rutId);
    var minLengthRut = 7;
    $rut.rut({ formatOn: 'keyup change', minimumLength: minLengthRut, validateOn: 'change' });
    $.validator.addMethod("rut", function (value, element, _params) {
        $(element).val(value.replace(/k/, "K"));
        var valid = false;
        $.validateRut(value, function (rut, _dv) {
            if (rut > 30000000)
                return;
            valid = true;
        }, { minimumLength: minLengthRut });
        return valid;
    });
    $.validator.unobtrusive.adapters.add("rut", [], function (options) {
        options.rules.rut = {};
        options.messages["rut"] = options.message;
    });
});
//# sourceMappingURL=add_rut_validation.js.map