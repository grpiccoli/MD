document.addEventListener('DOMContentLoaded', () => {
    $('form[data-parsley-validate]').parsley({
        trigger: 'change'
    });

    //$('form[data-parsley-validate]').submit(function (e) {
    //    var $form = $(this);
    //    var valid = $form.parsley().validate();
    //    console.log($form, valid, $form.parsley().isValid());
    //    if ($form.parsley().isValid()) {
    //        //$form.submit();
    //    } else {
    //        e.preventDefault();
    //    }
    //});

    $('select').change(_ => $('form').validate().element('select'));

    $('select').css({
        display: "block",
        position: 'absolute',
        visibility: 'hidden'
    });

    $('.field-validation-valid').on('DOMSubtreeModified', function () {
        if ($(this).is(':empty')) {
            $(this).parent().children("input").removeClass('invalid');
            $(this).parent().children("input").addClass('valid');
        } else {
            $(this).parent().children("input").removeClass('valid');
            $(this).parent().children("input").addClass('invalid');
        }
    });
});

