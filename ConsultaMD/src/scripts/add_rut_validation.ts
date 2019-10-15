//VARIABLES
var rutId = "Input_RUT";
var $rut = $(`#${rutId}`);
var minLengthRut = 7;

//Format as you type RUT
$rut.rut({ formatOn: 'keyup change', minimumLength: minLengthRut, validateOn: 'change' });

//Validator RUT
$.validator.addMethod("rut",
    function (value, element, _params) {
        $(element).val(value.replace(/k/, "K"));
        var valid = false;
        $.validateRut(value, function (rut, _dv) {
            if (rut as number > 30_000_000) return;
            valid = true;
        }, { minimumLength: minLengthRut });
        return valid;
    });

$.validator.unobtrusive.adapters.add("rut", [], function (options: any) {
    options.rules.rut = {};
    options.messages["rut"] = options.message;
});