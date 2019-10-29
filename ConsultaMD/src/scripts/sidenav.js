document.addEventListener('DOMContentLoaded', function () {
    M.Sidenav.init(document.querySelectorAll('.sidenav'));
    $("#editProfile").click(function () {
        var _this = this;
        var $el = $(this);
        if ($el.hasClass('teal')) {
            setTimeout(function () {
                $el.removeClass('teal').addClass('red').find('i').html("close");
                $('.phone i, .email i').addClass('edit').html('create').click(function () {
                    $(_this).next().prop('disabled', function (i, v) {
                        return !v;
                    });
                });
            }, 500);
        }
        else {
            setTimeout(function () {
                $el.removeClass('red').addClass('teal').find('i').html("create");
                $('.phone i.edit').unbind('click').removeClass('edit').html('phone');
                $('.email i.edit').unbind('click').removeClass('edit').html('mail');
            }, 500);
        }
    });
});
//# sourceMappingURL=sidenav.js.map