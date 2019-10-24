$(document).ready(function () {
    $('#modal1').modal();
    $(".paymentType").on('change', function () {
        var type = $(this).val();
        $("#PaymentType").val(type);
    });
});
//# sourceMappingURL=confirm-date.js.map