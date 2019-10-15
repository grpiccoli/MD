$.validator.setDefaults({
    errorClass: 'input-validation-error',
    validClass: "valid"
});

$("#form").validate({
    rules: {
        dateField: {
            date: true
        }
    }
});

$('select').on('change', function () {
    $('form').validate().element('select');
});

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