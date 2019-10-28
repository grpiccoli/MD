//VARIABLES
var rutId = "Input_RUT";
const nameId = "Input_Name";
const carnetId = "Input_CarnetId";
const nationId = "Input_Nationality";
var $rut = $(`#${rutId}`);
var $name = $(`#${nameId}`);
var $carnet = $(`#${carnetId}`);
var $nation = $(`#${nationId}`);
var minLengthRut = 7;
const minLengthCarnet = 9;
const debounceTO = 300;
const $nameDiv = $name.parent();
const $carnetDiv = $carnet.parent();
const $nationDiv = $nation.parent();

document.addEventListener('DOMContentLoaded', function () {
    var elems = document.querySelectorAll('.collapsible');
    var instances = M.Collapsible.init(elems, {});
});
//$('.collapsible').collapsible();

//Format as you type RUT
$rut.on('keyup', function () {
    $nameDiv.slideUp();
    $carnetDiv.slideUp();
    $nationDiv.slideUp();
    $name.prop('readonly', true);
    $carnet.val('');
    $name.val('');
}).rut({ formatOn: 'keyup change', minimumLength: minLengthRut, validateOn: 'change' });

//Format as you type Carnet
new Cleave(`#${carnetId}`, {
    delimiter: '.',
    blocks: [3, 3, 3],
    uppercase: true
})

//Validator RUT
$.validator.addMethod("rut",
    function (value, element, _params) {
        var valid = false;
        $(element).val(value.replace(/k/, "K"));
        $.validateRut(value, (rut: number, dv: string) => {
            if (rut as number > 30_000_000) return;
            if ($(":focus")[0] === $(element)[0]) { valid = true; return; }
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
                    } else {
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

$.validator.unobtrusive.adapters.add("rut", [], function (options: any) {
    options.rules.rut = {};
    options.messages["rut"] = options.message;
});

//Validator Carnet
$.validator.addMethod("carnet",
    function (value, element, _params) {
        var carnet = value;
        var valid = false;
        $nationDiv.slideUp();
        var rutVal = $rut.val() as string;
        if (rutVal) {
            $.validateRut(rutVal, function (rut, dv) {
                if ($(":focus")[0] === $(element)[0]) { valid = true; return; }
                $.ajax({
                    method: 'POST',
                    url: '/SP/DocumentRequestStatus',
                    async: false,
                    data: {
                        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
                        rut: rut,
                        dv: dv.replace(/k/g, "K"),
                        carnet: carnet.replace(/\./g,"")
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

$.validator.unobtrusive.adapters.add("carnet", [], function (options: any) {
    options.rules.carnet = {};
    options.messages["carnet"] = options.message;
});