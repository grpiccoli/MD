$(".external-login").on('click', function () {
    $.post("./ExternalLogin", { returnUrl: $(this).data('returnurl'), provider: $(this).val });
});