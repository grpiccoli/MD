//VARIABLES
var rutId = "Rut";
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

document.addEventListener('DOMContentLoaded', () => {
    $('#Payment').on('submit', (e) => {
        loaderStart();
        var waiting = setInterval(() => {
            M.toast({ html: 'Comprando bono y redirigiendo a pago, esto puede tardar varios minutos' })
        }, 10 * 1000);
        e.preventDefault();
        var url = $('#Payment').attr('action');
        console.log(url);
        var type = $('#PaymentDetails_Type').val();
        $.post(url, $('#Payment').serializeArray(), response => {
            console.log(response);
            switch (type) {
                case '0':
                    window.location.href = response;
                    break;
                case '1':
                    window.location.href = 'https://webpay3g.transbank.cl:443/webpayserver/initTransaction?token_ws=' + response;
                    break;
                default:
                    alert('error inesperado');
                    break;
            };
            loaderStop();
            clearInterval(waiting);
        });
    });
    //var elems = document.getElementById('modal1');
    //var instances = M.Modal.init(elems, {});
    //$('#Dia').datepicker({
    //    format: 'dd mmmm, yyyy',
    //    i18n: {
    //        months: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
    //        monthsShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Set", "Oct", "Nov", "Dic"],
    //        weekdays: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
    //        weekdaysShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
    //        weekdaysAbbrev: ["D", "L", "M", "M", "J", "V", "S"],
    //        cancel: 'Cancelar',
    //        clear: 'Limpar',
    //        done: 'Ok'
    //    },
    //    onOpen: function () {
    //        this.close();
    //    }
    //});

    //$('#Pay').click(() => {
    //    $.post('../Payment', $('#Payment').serializeArray(), response => {
    //        var json = response.json()
    //        var _idFrm = 'frm' + (new Date).getTime();
    //        $(document.body).append('<form action="https://webpay3g.transbank.cl:443/webpayserver/initTransaction" method="post" id="' + _idFrm + '"></form>');
    //        $('#' + _idFrm).append('<input type="hidden" name="token_ws" value="' + json.tokenWs + '" />');
    //        $('#' + _idFrm).submit();
    //    });
    //});

    //$(".paymentType").on('change', function () {
    //    var type = $(this).val();
    //    $("#PaymentType").val(type);
    //});
});