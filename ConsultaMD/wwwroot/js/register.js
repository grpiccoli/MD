var rutId = "Input_RUT";
var nameId = "Input_Name";
var carnetId = "Input_CarnetId";
var nationId = "Input_Nationality";
var $rut = $("#" + rutId);
var $name = $("#" + nameId);
var $carnet = $("#" + carnetId);
var $nation = $("#" + nationId);
var minLengthRut = 7;
var minLengthCarnet = 9;
var debounceTO = 300;
var $nameDiv = $name.parent();
var $carnetDiv = $carnet.parent();
var $nationDiv = $nation.parent();
$('.collapsible').collapsible();
$rut.on('keyup', function () {
    $nameDiv.slideUp();
    $carnetDiv.slideUp();
    $nationDiv.slideUp();
    $name.prop('readonly', true);
    $carnet.val('');
    $name.val('');
}).rut({ formatOn: 'keyup change', minimumLength: minLengthRut, validateOn: 'change' });
new Cleave("#" + carnetId, {
    delimiter: '.',
    blocks: [3, 3, 3],
    uppercase: true
});
$.validator.addMethod("rut", function (value, element, _params) {
    var valid = false;
    $(element).val(value.replace(/k/, "K"));
    $.validateRut(value, function (rut, dv) {
        if (rut > 30000000)
            return;
        if ($(":focus")[0] === $(element)[0]) {
            valid = true;
            return;
        }
        $.ajax({
            method: 'POST',
            url: '/SP/validaterut',
            async: false,
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                rut: rut,
                dv: dv.replace(/k/, "K")
            },
            dataType: 'JSON',
            success: function (result) {
                valid = true;
                if (result.value === "**") {
                    $name.prop('readonly', false);
                }
                else {
                    $name.val(result.value);
                    $name.addClass('valid');
                }
                M.updateTextFields();
                $nameDiv.slideDown();
                $carnetDiv.slideDown();
                $carnet.focus();
            }
        });
    }, { minimumLength: minLengthRut });
    return valid;
});
$.validator.unobtrusive.adapters.add("rut", [], function (options) {
    options.rules.rut = {};
    options.messages["rut"] = options.message;
});
$.validator.addMethod("carnet", function (value, element, _params) {
    var carnet = value;
    var valid = false;
    $nationDiv.slideUp();
    var rutVal = $rut.val();
    if (rutVal) {
        $.validateRut(rutVal, function (rut, dv) {
            if ($(":focus")[0] === $(element)[0]) {
                valid = true;
                return;
            }
            $.ajax({
                method: 'POST',
                url: '/SP/DocumentRequestStatus',
                async: false,
                data: {
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                    rut: rut,
                    dv: dv.replace(/k/g, "K"),
                    carnet: carnet.replace(/\./g, "")
                },
                dataType: 'JSON',
                success: function (result) {
                    valid = true;
                    $nation.val(result.nacionalidad);
                    M.updateTextFields();
                    $nationDiv.slideDown();
                }
            });
        }, { minimumLength: minLengthRut });
    }
    return valid;
});
$.validator.unobtrusive.adapters.add("carnet", [], function (options) {
    options.rules.carnet = {};
    options.messages["carnet"] = options.message;
});
//# sourceMappingURL=add_rut_carnet_validation.js.map