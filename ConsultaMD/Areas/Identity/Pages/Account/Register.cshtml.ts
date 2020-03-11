document.addEventListener('DOMContentLoaded', _ => {
    //VARIABLES
    var rutId = "Input_RUT";
    const nameId = "Input_Name";
    const carnetId = "Input_CarnetId";
    //const nationId = "Input_Nationality";
    const email = "Input_Email";
    var $rut = $(`#${rutId}`);
    var $name = $(`#${nameId}`);
    var $carnet = $(`#${carnetId}`);
    //var $nation = $(`#${nationId}`);
    var $email = $(`#${email}`);
    var minLengthRut = 7;
    const $nameDiv = $name.parent();
    const $carnetDiv = $carnet.parent();
    //const $nationDiv = $nation.parent();

    //Format as you type Carnet
    new Cleave(`#${carnetId}`, {
        delimiter: '.',
        blocks: [3, 3, 3],
        uppercase: true
    });

    $.validator.addMethod('rut',
        (value, element, _params) => {
            $(element).val(value.replace(/k/, "K"));
            return $.validateRut(value, function (r, d) {
                if (r > 30_000_000) return false;
            }, { minimumLength: minLengthRut });
            //loaderStart();
            //return fetch('/SP/validaterut', {
            //    method: 'POST',
            //    headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
            //    body: `__RequestVerificationToken=${$('input[name="__RequestVerificationToken"]').val()}`
            //        + `&rut=${rut}`
            //        + `&dv=${dv.replace(/k/, 'K')}`
            //})
            //    .then(response => response.json())
            //    .then((json: any) => {
            //        $name.val(json.value);
            //        $name.addClass('valid');
            //        M.updateTextFields();
            //        $nameDiv.slideDown();
            //        $carnetDiv.slideDown();
            //        $carnet.focus();
            //        M.toast({ html: 'RUT válido', classes: 'rounded' });
            //        $rut.parsley().reset();
            //        return true;
            //    })
            //    .catch(err => {
            //        $rut.addError("rut", { message: "RUT inválido", updateClass: true });
            //        throw (err);
            //    })
            //    .finally(loaderStop);
        });

    $.validator.addMethod('carnet',
        (value, element, _params) => {
            //$nationDiv.slideUp();
            var rutVal = $rut.val() as string;
            if (!rutVal) return false;
            var carnet = parseInt(value.replace(/\./g, ""));
            return $.validateRut(value, function (r, d) {
                if (r > 30_000_000) return false;
            }, { minimumLength: minLengthRut })
            && (carnet < 100_000_000 || carnet > 999_999_999);
            //loaderStart();
            //var isExt = $("#IsExt").prop('checked');
            //var carnetValid = fetch('/SP/DocumentRequestStatus', {
            //    method: 'POST',
            //    headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
            //    body: `__RequestVerificationToken=${$('input[name="__RequestVerificationToken"]').val()}`
            //        + `&rut=${rut}`
            //        + `&dv=${dv.replace(/k/, 'K')}`
            //        + `&carnet=${value.replace(/\./g, "")}`
            //        + `&isExt=${isExt}`
            //})
            //    .then(response => response.text())
            //    .then((bool: any) => {
            //        //$nation.val(json.nacionalidad);
            //        M.updateTextFields();
            //        //$nationDiv.slideDown();
            //        var rValid = bool === 'true';
            //        if (rValid) {
            //            $email.focus();
            //            M.toast({ html: `Cédula válida`, classes: `rounded` });
            //            $carnet.parsley().reset();
            //        } else {
            //            M.toast({ html: `Cédula inválida`, classes: `rounded danger` });
            //            $rut.addError("carnet", { message: "Cédula inválida", updateClass: true });
            //        }
            //        return rValid;
            //    })
            //    .catch(err => {
            //        $rut.addError("carnet", { message: "Error al validar Carnét", updateClass: true });
            //        throw (err);
            //    })
            //    .finally(loaderStop);
            //console.log(carnetValid);
            //return carnetValid;
        });

    //Format as you type RUT
    $rut
        //.keyup(_ => {
        //$nameDiv.slideUp();
        //$carnetDiv.slideUp();
        //$nationDiv.slideUp();
        //$name.prop('readonly', true);
        //$carnet.val('');
        //$name.val('');
    //})
        .rut({ formatOn: 'keyup change', minimumLength: minLengthRut, validateOn: 'change' });

    $.validator.unobtrusive.adapters.add('carnet', [], (options: any) => {
        options.rules.rut = {};
        options.messages['carnet'] = options.message;
    });

    $.validator.unobtrusive.adapters.add("rut", [], (options: any) => {
        options.rules.rut = {};
        options.messages["rut"] = options.message;
    });

    M.Collapsible.init(document.querySelectorAll('.collapsible'));
});