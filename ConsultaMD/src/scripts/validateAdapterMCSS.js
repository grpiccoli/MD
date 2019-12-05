document.addEventListener('DOMContentLoaded', function () {
    $('form[data-parsley-validate]').parsley({
        trigger: 'change'
    });
    $('select').change(function (_) { return $('form').validate().element('select'); });
    $('select').css({
        display: "block",
        position: 'absolute',
        visibility: 'hidden'
    });
    $('.field-validation-valid').on('DOMSubtreeModified', function () {
        if ($(this).is(':empty')) {
            $(this).parent().children("input").removeClass('invalid');
            $(this).parent().children("input").addClass('valid');
        }
        else {
            $(this).parent().children("input").removeClass('valid');
            $(this).parent().children("input").addClass('invalid');
        }
    });
});
//# sourceMappingURL=validateAdapterMCSS.js.map