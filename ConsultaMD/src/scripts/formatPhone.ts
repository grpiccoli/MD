$.validator.addMethod("cell",
    function (value, element, _params) {
        var valid = false;
        var $obj = $(element);
        var val_old = value.replace(/\D/g, '').trim();
        if (val_old.length > 1) {
            if (val_old.length != 9) {
                $obj.val(new libphonenumber.AsYouType('CL').input(val_old));
                return;
            }
            var phoneNumber = libphonenumber.parsePhoneNumberFromString(val_old, 'CL');
            if (phoneNumber) {
                if (phoneNumber.isValid()) {
                    console.log(phoneNumber.getType());
                    var phone = phoneNumber.formatNational();
                    $obj.val(phone);
                    if (/9\s[1-9]\d{3}\s[0-9]{4}/.test(phone))
                        valid = true;
                }
            }
        }
        return valid;
    });

$.validator.unobtrusive.adapters.add("cell", [], function (options: any) {
    options.rules.cell = {};
    options.messages["cell"] = options.message;
});

//mobiscroll.settings = {
//    lang: 'es',
//    theme: 'ios',
//    themeVariant: 'light'
//};

//mobiscroll.numpad('#PhoneNumber', {
//    fill: 'ltr',
//    template: '(+569) dddd dddd',
//    allowLeadingZero: false,
//    validate: function (event, inst) {
//        if (inst.isVisible()) {
//            // Display the formatted value
//            inst._markup[0].querySelector('.mbsc-np-dsp').innerHTML =
//                inst.settings.formatValue(event.values, event.variables, inst) || '&nbsp;';
//        }

//        return {
//            // Set the set button invalid until 10 chars filled
//            invalid: event.values.length != 10
//        };
//    },
//    formatValue: function (numbers, _variables, _inst) {
//        // Specify how the value will be formatted
//        return (numbers.length > 2 ? '(' : '') + numbers.slice(0, 3).join('')
//            + (numbers.length > 2 ? ') ' : '') + numbers.slice(3, 6).join('')
//            + (numbers.length > 5 ? '-' : '') + numbers.slice(6, 10).join('');
//    },
//    parseValue: function (v) {
//        if (v) {
//            return v.replace('(', '').replace(') ', '').replace('-', '');
//        }
//    }
//});