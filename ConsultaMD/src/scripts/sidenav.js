document.addEventListener('DOMContentLoaded', function () {
    M.Sidenav.init(document.querySelectorAll('.sidenav'));
    $("#editProfile").click(function () {
        var $el = $(this);
        if ($el.hasClass('teal')) {
            setTimeout(function () {
                $el.removeClass('teal').addClass('red').find('i').html("close");
                $('.phone i, .email i').addClass('edit').html('create');
                $('.phone i.edit').click(function () {
                    window.location.href = '/Identity/Account/VerifyPhone?returnUrl=%2FPatients%2FSearch%2FMap';
                });
                $('.email i.edit').click(function () {
                });
            }, 500);
        }
        else {
            setTimeout(function () {
                $el.removeClass('red').addClass('teal').find('i').html("create");
                $('.phone i').removeClass('edit').html('phone');
                $('.email i').removeClass('edit').html('mail');
            }, 500);
        }
    });
});
//# sourceMappingURL=sidenav.js.map