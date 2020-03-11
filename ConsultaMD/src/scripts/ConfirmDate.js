var rutId = "Rut";
var $rut = $("#" + rutId);
var minLengthRut = 7;
$rut.rut({ formatOn: 'keyup change', minimumLength: minLengthRut, validateOn: 'change' });
$.validator.addMethod("rut", function (value, element, _params) {
    $(element).val(value.replace(/k/, "K"));
    var valid = false;
    $.validateRut(value, function (rut, _dv) {
        if (rut > 30000000)
            return;
        valid = true;
    }, { minimumLength: minLengthRut });
    return valid;
});
$.validator.unobtrusive.adapters.add("rut", [], function (options) {
    options.rules.rut = {};
    options.messages["rut"] = options.message;
});
document.addEventListener('DOMContentLoaded', function () {
    $('#Payment').on('submit', function (e) {
        loaderStart();
        var waiting = setInterval(function () {
            M.toast({ html: 'Comprando bono y redirigiendo a pago, esto puede tardar varios minutos' });
        }, 10 * 1000);
        e.preventDefault();
        var url = $('#Payment').attr('action');
        console.log(url);
        var type = $('#PaymentDetails_Type').val();
        $.post(url, $('#Payment').serializeArray(), function (response) {
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
            }
            ;
            loaderStop();
            clearInterval(waiting);
        });
    });
});
//# sourceMappingURL=ConfirmDate.js.map