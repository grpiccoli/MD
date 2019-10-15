$.validator.addMethod("cell", function (value, element, _params) {
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
$.validator.unobtrusive.adapters.add("cell", [], function (options) {
    options.rules.cell = {};
    options.messages["cell"] = options.message;
});
//# sourceMappingURL=formatPhone.js.map