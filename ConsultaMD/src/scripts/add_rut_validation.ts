document.addEventListener('DOMContentLoaded', _ => {
    //VARIABLES
    var rutId = "Input_RUT";
    var $rut = $(`#${rutId}`);
    var minLengthRut = 7;

    //Format as you type RUT
    $rut.rut({ formatOn: 'keyup change', minimumLength: minLengthRut, validateOn: 'change' });

    //Validator RUT
    $.validator.addMethod("rut",
        (value, element, _params) => {
            $(element).val(value.replace(/k/, "K"));
            return $.validateRut(value, function (r, d) {
                if (r > 30_000_000) return false;
            }, { minimumLength: minLengthRut });
        });

    $.validator.unobtrusive.adapters.add("rut", [], (options: any) => {
        options.rules.rut = {};
        options.messages["rut"] = options.message;
    });
});