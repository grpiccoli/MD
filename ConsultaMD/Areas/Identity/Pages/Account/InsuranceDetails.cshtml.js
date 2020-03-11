document.addEventListener('DOMContentLoaded', function () {
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
        "https://www.isaprebanmedica.cl/CambiodeClaveBanmedica.aspx",
        "https://www.colmena.cl/web-administracion/sitioprivado/solicitudclave/solicitudclave.jsp",
        "https://clientes.consalud.cl/default.aspx#",
        "https://sitio.cruzblanca.cl/MiCruzBlanca/RecuperacionClave.aspx",
        "https://sv.nuevamasvida.cl/solicitud_clave/?url=recupera",
        "https://www.isaprevidatres.cl/CambioDeClaveVidaTres.aspx"
    ];
    $insuranceId.change(function () {
        $insurancePass.val('');
        var val = parseInt($('#Input_Insurance').val());
        if (val === 0) {
            $insurancePassDiv.slideUp();
        }
        else {
            var text = $("#Input_Insurance_dummy").val();
            $recovermip.attr("href", mi[val - 2]);
            $recovery.attr('src', "/img/mi/" + text + "-icon.min.png");
            $insurancePassDiv.slideDown();
        }
    });
    Parsley.addValidator('mipwd', {
        validateString: function (value) {
            var insurance = $insuranceId.val();
            if (insurance === '0')
                return true;
            var rutVal = $rut.val();
            if (!rutVal)
                return false;
            var rut, dv;
            var valid = $.validateRut(rutVal, function (r, d) {
                rut = r;
                dv = d;
            }, { minimumLength: minLengthRut });
            if (!valid || rut > 30000000)
                return false;
            loaderStart();
            return fetch('/MIP/Validate', {
                method: 'POST',
                headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
                body: "__RequestVerificationToken=" + $('input[name="__RequestVerificationToken"]').val()
                    + ("&rut=" + rut)
                    + ("&dv=" + dv.replace(/k/, 'K'))
                    + ("&insurance=" + insurance)
                    + ("&pwd=" + value)
            })
                .then(function (response) {
                if (response.ok) {
                    M.toast({ html: 'Contraseña válida', classes: 'rounded' });
                    $recovermip.slideUp();
                    $info.slideUp();
                    $insurancePass.parsley().reset();
                    return true;
                }
                throw (response.statusText);
            })
                .catch(function (err) {
                $recovermip.slideDown();
                $info.slideDown();
                throw (err);
            })
                .finally(loaderStop);
        },
        messages: { es: 'Verifique Combinación Previsión/Contraseña' }
    });
});
//# sourceMappingURL=InsuranceDetails.cshtml.js.map