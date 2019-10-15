const insuranceId = "Input_Insurance";
const insurancePass = "Input_InsurancePassword";
var rutId = "Input_RUT";
const recovery = "recovery";
const info = "info";
const $insuranceId = $(`#${insuranceId}`);
const $insurancePass = $(`#${insurancePass}`);
var $rut = $(`#${rutId}`);
const $recovermip = $("#recoverpass");
const $insurancePassDiv = $insurancePass.parent();
const $recovery = $(`#${recovery}`)
const $info = $(`#${info}`)
var minLengthRut = 7;

//urls of insurance password recovery
const mi = [
    "#", "#",
    "https://www.isaprebanmedica.cl/CambiodeClaveBanmedica.aspx",
    "https://www.colmena.cl/web-administracion/sitioprivado/solicitudclave/solicitudclave.jsp",
    "https://clientes.consalud.cl/default.aspx#",
    "https://sitio.cruzblanca.cl/MiCruzBlanca/RecuperacionClave.aspx",
    "https://sv.nuevamasvida.cl/solicitud_clave/?url=recupera",
    "https://www.isaprevidatres.cl/CambioDeClaveVidaTres.aspx"
]

//change image and url for insuarance password recovery
function switchRecovery(val : number, $e: any) {
    $recovermip.attr("href", mi[val]);
    $recovery.attr('src', `/img/mi/${$e.text()}-icon.min.png`);
}

//add images to select
$(`#${insuranceId} > option`).slice(1).each(function () {
    var $this = $(this);
    $this.attr('data-icon', `/img/mi/${$this.text()}-icon.min.png`);
});

//initialize select
$('select').formSelect();

//Validate User / Insurance / pwd combo
function validateInsurance(val: number, rut: number, dv: string, pwd: string): boolean {
    var ret = false;
    debounce($.ajax({
        method: 'POST',
        url: `/MIP/Validate`,
        async: false,
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            insurance: val,
            rut: rut,
            dv: dv,
            pwd: pwd
        },
        success: function (r) {
            $recovermip.slideUp();
            $info.slideUp();
            ret = true;
        },
        error: function (e, _xhr, _ex) {
            $recovermip.slideDown();
            $info.slideDown();
        }
    }), 1000);
    return ret;
}

$insuranceId.on('change', function () {
    $insurancePass.val('');
});

//Validate Insuarence select
$.validator.addMethod("insurance", function (value, element, _params) {
    var valid = true;
    var $this = $(element);
    var rutVal = $rut.val() as string;
    if (rutVal) {
        $.validateRut(rutVal, function (rut, dv) {
            var val = parseInt(value as string);
            switch (val) {
                default:
                    switchRecovery(val, $this);
                case 1:
                    valid = validateInsurance(val, rut, dv.replace(/k/,"K"), '');
                case 0:
                    $insurancePassDiv.slideUp();
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    $insurancePassDiv.slideDown();
                    break;
            }
        }, { minimumLength: minLengthRut });
    }
    return valid;
});

$.validator.unobtrusive.adapters.add("insurance", [], function (options: any) {
    options.rules.insurance = {};
    options.messages["insurance"] = options.message;
});

//Validate pwd
$.validator.addMethod("mipwd", function (value, element, _params) {
    var insurance = $insuranceId.val();
    if (insurance === '0' || insurance === '1') { return true; }
    var valid = false;
    var rutVal = $rut.val() as string;
    if (rutVal) {
        $.validateRut(rutVal, function (rut, dv) {
            if ($(":focus")[0] === $(element)[0]) { valid = true; return; }
            var prev = parseInt(insurance as string);
            console.log(prev, rut, dv, value);
            valid = validateInsurance(prev, rut, dv, value);
        }, { minimumLength: minLengthRut });
    }
    return valid;
});

$.validator.unobtrusive.adapters.add("mipwd", [], function (options: any) {
    options.rules.mipwd = {};
    options.messages["mipwd"] = options.message;
});