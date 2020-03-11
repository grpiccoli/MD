document.addEventListener('DOMContentLoaded', function () {
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
    var mi = [
        "https://www.isaprebanmedica.cl/CambiodeClaveBanmedica.aspx",
        "https://www.colmena.cl/web-administracion/sitioprivado/solicitudclave/solicitudclave.jsp",
        "https://clientes.consalud.cl/default.aspx#",
        "https://sitio.cruzblanca.cl/MiCruzBlanca/RecuperacionClave.aspx",
        "https://sv.nuevamasvida.cl/solicitud_clave/?url=recupera",
        "https://www.isaprevidatres.cl/CambioDeClaveVidaTres.aspx"
    ];

    $insuranceId.change(() => {
        $insurancePass.val('');
        var val = parseInt($('#Input_Insurance').val() as string);
        if (val === 0) {
            $insurancePassDiv.slideUp();
        } else {
            var text = $("#Input_Insurance_dummy").val();
            //change image and url for insurance password recovery
            $recovermip.attr("href", mi[val - 2]);
            $recovery.attr('src', `/img/mi/${text}-icon.min.png`);
            $insurancePassDiv.slideDown();
        }
    });

    //Validate pwd
    Parsley.addValidator('mipwd', {
        validateString: (value: string) => {
            var insurance = $insuranceId.val();
            if (insurance === '0') return true;
            var rutVal = $rut.val() as string;
            if (!rutVal) return false;
            var rut: number, dv: string;
            var valid = $.validateRut(rutVal, (r: number, d: string) => {
                rut = r;
                dv = d;
            }, { minimumLength: minLengthRut });
            if (!valid || rut > 30_000_000) return false;
            loaderStart();
            return fetch('/MIP/Validate', {
                method: 'POST',
                headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
                body: `__RequestVerificationToken=${$('input[name="__RequestVerificationToken"]').val()}`
                    + `&rut=${rut}`
                    + `&dv=${dv.replace(/k/, 'K')}`
                    + `&insurance=${insurance}`
                    + `&pwd=${value}`
            })
                .then(response => {
                    if (response.ok) {
                        M.toast({ html: 'Contraseña válida', classes: 'rounded' });
                        $recovermip.slideUp();
                        $info.slideUp();
                        $insurancePass.parsley().reset();
                        return true;
                    }
                    throw(response.statusText);
                })
                .catch(err => {
                    $recovermip.slideDown();
                    $info.slideDown();
                    throw(err);
                })
                .finally(loaderStop);
        },
        messages: { es: 'Verifique Combinación Previsión/Contraseña' }
    });
});