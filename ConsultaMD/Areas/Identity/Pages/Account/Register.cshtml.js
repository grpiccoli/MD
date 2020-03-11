document.addEventListener('DOMContentLoaded', function (_) {
    var rutId = "Input_RUT";
    var nameId = "Input_Name";
    var carnetId = "Input_CarnetId";
    var email = "Input_Email";
    var $rut = $("#" + rutId);
    var $name = $("#" + nameId);
    var $carnet = $("#" + carnetId);
    var $email = $("#" + email);
    var minLengthRut = 7;
    var $nameDiv = $name.parent();
    var $carnetDiv = $carnet.parent();
    new Cleave("#" + carnetId, {
        delimiter: '.',
        blocks: [3, 3, 3],
        uppercase: true
    });
    $.validator.addMethod('rut', function (value, element, _params) {
        $(element).val(value.replace(/k/, "K"));
        return $.validateRut(value, function (r, d) {
            if (r > 30000000)
                return false;
        }, { minimumLength: minLengthRut });
    });
    $.validator.addMethod('carnet', function (value, element, _params) {
        var rutVal = $rut.val();
        if (!rutVal)
            return false;
        var carnet = parseInt(value.replace(/\./g, ""));
        return $.validateRut(value, function (r, d) {
            if (r > 30000000)
                return false;
        }, { minimumLength: minLengthRut })
            && (carnet < 100000000 || carnet > 999999999);
    });
    $rut
        .rut({ formatOn: 'keyup change', minimumLength: minLengthRut, validateOn: 'change' });
    $.validator.unobtrusive.adapters.add('carnet', [], function (options) {
        options.rules.rut = {};
        options.messages['carnet'] = options.message;
    });
    $.validator.unobtrusive.adapters.add("rut", [], function (options) {
        options.rules.rut = {};
        options.messages["rut"] = options.message;
    });
    M.Collapsible.init(document.querySelectorAll('.collapsible'));
});
//# sourceMappingURL=Register.cshtml.js.map