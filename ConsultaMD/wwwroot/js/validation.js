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
    }
    else {
        $(this).parent().children("input").removeClass('valid');
        $(this).parent().children("input").addClass('invalid');
    }
});
//# sourceMappingURL=validateAdapterMCSS.js.map
(function ($) {
    var culture = navigator.language;
    $.when($.get("/lib/cldr-data/supplemental/likelySubtags.json"), $.get("/lib/cldr-data/main/" + culture + "/numbers.json"), $.get("/lib/cldr-data/supplemental/numberingSystems.json"), $.get("/lib/cldr-data/main/" + culture + "/ca-gregorian.json"), $.get("/lib/cldr-data/main/" + culture + "/timeZoneNames.json"), $.get("/lib/cldr-data/supplemental/timeData.json"), $.get("/lib/cldr-data/supplemental/weekData.json")).then(function () {
        return [].slice.apply(arguments, [0]).map(function (result) {
            return result[0];
        });
    }).then(Globalize.load).then(function () {
        if (culture) {
            Globalize.locale(culture);
        }
    });
})(jQuery);
//# sourceMappingURL=validate-localization.js.map