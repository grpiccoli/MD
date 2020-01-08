document.addEventListener('DOMContentLoaded', function (_) {
    var rutId = "Input_RUT";
    var nameId = "Input_Name";
    var carnetId = "Input_CarnetId";
    var email = "Input_Email";
    var $rut = $("#" + rutId);
    var $name = $("#" + nameId);
    var $carnet = $("#" + carnetId);
    var $email = $("#" + email);
    var minLengthRut = 7;
    var $nameDiv = $name.parent();
    var $carnetDiv = $carnet.parent();
    new Cleave("#" + carnetId, {
        delimiter: '.',
        blocks: [3, 3, 3],
        uppercase: true
    });
    Parsley.addValidator('rut', {
        validateString: function (value) {
            $rut.val(value.replace(/k/, "K"));
            var rut, dv;
            var valid = $.validateRut(value, function (r, d) {
                rut = r;
                dv = d;
            }, { minimumLength: minLengthRut });
            if (!valid || rut > 30000000)
                return false;
            loaderStart();
            return fetch('/SP/validaterut', {
                method: 'POST',
                headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
                body: "__RequestVerificationToken=" + $('input[name="__RequestVerificationToken"]').val()
                    + ("&rut=" + rut)
                    + ("&dv=" + dv.replace(/k/, 'K'))
            })
                .then(function (response) { return response.json(); })
                .then(function (json) {
                $name.val(json.value);
                $name.addClass('valid');
                M.updateTextFields();
                $nameDiv.slideDown();
                $carnetDiv.slideDown();
                $carnet.focus();
                M.toast({ html: 'RUT válido', classes: 'rounded' });
                $rut.parsley().reset();
                return true;
            })
                .catch(function (err) {
                $rut.addError("rut", { message: "RUT inválido", updateClass: true });
                throw (err);
            })
                .finally(loaderStop);
        },
        messages: { es: 'RUT inválido' }
    });
    Parsley.addValidator('carnet', {
        validateString: function (value) {
            var rutVal = $rut.val();
            if (!rutVal)
                return false;
            var rut, dv;
            var valid = $.validateRut(rutVal, function (r, d) {
                rut = r;
                dv = d;
            }, { minimumLength: minLengthRut });
            if (!valid || rut > 30000000)
                return false;
            loaderStart();
            var isExt = $("#IsExt").prop('checked');
            var carnetValid = fetch('/SP/DocumentRequestStatus', {
                method: 'POST',
                headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
                body: "__RequestVerificationToken=" + $('input[name="__RequestVerificationToken"]').val()
                    + ("&rut=" + rut)
                    + ("&dv=" + dv.replace(/k/, 'K'))
                    + ("&carnet=" + value.replace(/\./g, ""))
                    + ("&isExt=" + isExt)
            })
                .then(function (response) { return response.text(); })
                .then(function (bool) {
                M.updateTextFields();
                var rValid = bool === 'true';
                if (rValid) {
                    $email.focus();
                    M.toast({ html: "C\u00E9dula v\u00E1lida", classes: "rounded" });
                    $carnet.parsley().reset();
                }
                else {
                    M.toast({ html: "C\u00E9dula inv\u00E1lida", classes: "rounded danger" });
                    $rut.addError("carnet", { message: "Cédula inválida", updateClass: true });
                }
                return rValid;
            })
                .catch(function (err) {
                $rut.addError("carnet", { message: "Error al validar Carnét", updateClass: true });
                throw (err);
            })
                .finally(loaderStop);
            console.log(carnetValid);
            return carnetValid;
        },
        messages: { es: 'Verfique combinación RUT/Carnet' }
    });
    $rut.keyup(function (_) {
        $nameDiv.slideUp();
        $carnetDiv.slideUp();
        $name.prop('readonly', true);
        $carnet.val('');
        $name.val('');
    })
        .rut({ formatOn: 'keyup change', minimumLength: minLengthRut, validateOn: 'change' });
    M.Collapsible.init(document.querySelectorAll('.collapsible'));
});
//# sourceMappingURL=add_rut_carnet_validation.js.map