document.addEventListener('DOMContentLoaded', function () {
    M.Sidenav.init(document.querySelectorAll('.sidenav'));
    $("#editProfile").click(function () {
        var $el = $(this);
        if ($el.hasClass('teal')) {
            setTimeout(() => {
                $el.removeClass('teal').addClass('red').find('i').html("close");
                $('.phone i, .email i').addClass('edit').html('create');
                $('.phone i.edit').click(() => {
                    $('#userphone').prop('disabled', (i, v) => { return !v; });
                    //window.location.href = '/Identity/Account/VerifyPhone?returnUrl=%2FPatients%2FSearch%2FMap';
                });
                $('.email i.edit').click(() => {
                    $('#useremail').prop('disabled', (i, v) => { return !v; });
                });
            }, 500);
        } else {
            setTimeout(() => {
                $el.removeClass('red').addClass('teal').find('i').html("create");
                $('.phone i.edit').unbind('click').removeClass('edit').html('phone');
                $('.email i.edit').unbind('click').removeClass('edit').html('mail');
            }, 500);
        }
    });
});