var insuranceId = "Input_Insurance";
var insurancePass = "Input_InsurancePassword";
var rutId = "Input_RUT";
var recovery = "recovery";
var info = "info";
var $insuranceId = $("#" + insuranceId);
var $insurancePass = $("#" + insurancePass);
var $rut = $("#" + rutId);
var $recovermip = $("#recoverpass");
var $insurancePassDiv = $insurancePass.parent();
var $recovery = $("#" + recovery);
var $info = $("#" + info);
var minLengthRut = 7;
var mi = [
    "#", "#",
    "https://www.isaprebanmedica.cl/CambiodeClaveBanmedica.aspx",
    "https://www.colmena.cl/web-administracion/sitioprivado/solicitudclave/solicitudclave.jsp",
    "https://clientes.consalud.cl/default.aspx#",
    "https://sitio.cruzblanca.cl/MiCruzBlanca/RecuperacionClave.aspx",
    "https://sv.nuevamasvida.cl/solicitud_clave/?url=recupera",
    "https://www.isaprevidatres.cl/CambioDeClaveVidaTres.aspx"
];
function switchRecovery(val, $e) {
    $recovermip.attr("href", mi[val]);
    $recovery.attr('src', "/img/mi/" + $e.text() + "-icon.min.png");
}
if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|Nexus/i
    .test(navigator.userAgent)) {
    var selectdata = [];
    var style = "height:17px;margin:0 15px";
    $("#Input_Insurance > option").each(function () {
        selectdata.push("<img src=\"/img/mi/" + $(this).text() + "-icon.min.png\" style=\"" + style + "\" data-id=\"" + $(this).val() + "\"/><span>" + $(this).text() + "</span>");
    });
    $('select').remove();
    var selectModal = M.Modal.init(document.getElementById('select-modal'), {
        onCloseEnd: function () { $('#Input_Insurance').focusout(); }
    });
    $("#example-picker").picker({
        data: selectdata
    }, function () {
        var scrollAmmount = Math.round($('.clone-scroller').scrollTop() / 30);
        var $option = $($(".picker-scroller").find(".option").get(scrollAmmount));
        var id = $option.find('img').data('id');
        var text = $option.find('span').html();
        $('#Input_Insurance').val(id).change();
        $('#shower').val(text).change();
    });
    $('#shower').click(function () {
        selectModal.open();
    });
}
else {
    $("input.mobile-only").remove();
    $("#" + insuranceId + " > option").slice(1).each(function () {
        var $this = $(this);
        $this.attr('data-icon', "/img/mi/" + $this.text() + "-icon.min.png");
    });
    M.FormSelect.init(document.querySelectorAll('select'));
}
function validateInsurance(val, rut, dv, pwd) {
    var ret = false;
    debounce($.ajax({
        method: 'POST',
        url: "/MIP/Validate",
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
$insuranceId.on('change', function (_) {
    $insurancePass.val('');
});
$.validator.addMethod("insurance", function (value, element, _params) {
    var valid = true;
    var $this = $(element);
    var rutVal = $rut.val();
    if (rutVal) {
        $.validateRut(rutVal, function (r, dv) {
            var val = parseInt(value);
            switch (val) {
                default:
                    switchRecovery(val, $this);
                case 1:
                    valid = validateInsurance(val, r, dv.replace(/k/, "K"), '');
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
$.validator.unobtrusive.adapters.add("insurance", [], function (options) {
    options.rules.insurance = {};
    options.messages["insurance"] = options.message;
});
$.validator.addMethod("mipwd", function (value, element, _params) {
    var insurance = $insuranceId.val();
    if (insurance === '0' || insurance === '1') {
        return true;
    }
    var valid = false;
    var rutVal = $rut.val();
    if (rutVal) {
        $.validateRut(rutVal, function (r, dv) {
            if ($(":focus")[0] === $(element)[0]) {
                valid = true;
                return;
            }
            var prev = parseInt(insurance);
            valid = validateInsurance(prev, r, dv, value);
        }, { minimumLength: minLengthRut });
    }
    return valid;
});
$.validator.unobtrusive.adapters.add("mipwd", [], function (options) {
    options.rules.mipwd = {};
    options.messages["mipwd"] = options.message;
});
//# sourceMappingURL=selectInsurance.js.map