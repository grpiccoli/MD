document.addEventListener('DOMContentLoaded', function () {
    var elems = document.getElementById('modal1');
    var instances = M.Modal.init(elems, {});
    $(".paymentType").on('change', function () {
        var type = $(this).val();
        $("#PaymentType").val(type);
    });
});
//# sourceMappingURL=confirm-date.js.map