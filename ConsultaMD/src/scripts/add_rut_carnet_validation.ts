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

    Parsley.addValidator('rut', {
        validateString: (value: string) => {
            $rut.val(value.replace(/k/, "K"));
            var rut: number, dv: string;
            var valid = $.validateRut(value, (r: number, d: string) => {
                rut = r;
                dv = d;
            }, { minimumLength: minLengthRut });
            if (!valid || rut > 30_000_000) return false;
            loaderStart();
            return fetch('/SP/validaterut', {
                method: 'POST',
                headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
                body: `__RequestVerificationToken=${$('input[name="__RequestVerificationToken"]').val()}`
                    + `&rut=${rut}`
                    + `&dv=${dv.replace(/k/, 'K')}`
            })
                .then(response => response.json())
                .then((json: any) => {
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
                .catch(err => {
                    throw (err);
                })
                .finally(loaderStop);
        },
        messages: {es: 'RUT inválido'}
    });

    Parsley.addValidator('carnet', {
        validateString: (value: string) => {
            //$nationDiv.slideUp();
            var rutVal = $rut.val() as string;
            if (!rutVal) return false;
            var rut: number, dv: string;
            var valid = $.validateRut(rutVal, (r: number, d: string) => {
                rut = r;
                dv = d;
            }, { minimumLength: minLengthRut });
            if (!valid || rut > 30_000_000) return false;
            loaderStart();
            var isExt = $("#IsExt").prop('checked');
            return fetch('/SP/DocumentRequestStatus', {
                method: 'POST',
                headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
                body: `__RequestVerificationToken=${$('input[name="__RequestVerificationToken"]').val()}`
                    + `&rut=${rut}`
                    + `&dv=${dv.replace(/k/, 'K')}`
                    + `&carnet=${value.replace(/\./g, "")}`
                    + `&isExt=${isExt}`
            })
                .then(response => response.text())
                .then((bool: any) => {
                    console.log(bool);
                    //$nation.val(json.nacionalidad);
                    M.updateTextFields();
                    //$nationDiv.slideDown();
                    $email.focus();
                    M.toast({ html: 'Cédula válida', classes: 'rounded' });
                    $carnet.parsley().reset();
                    return true;
                })
                .catch(err => {
                    throw (err);
                })
                .finally(loaderStop);
        },
        messages: { es: 'Verfique combinación RUT/Carnet' }
    });

    //Format as you type RUT
    $rut.keyup(_ => {
        $nameDiv.slideUp();
        $carnetDiv.slideUp();
        //$nationDiv.slideUp();
        $name.prop('readonly', true);
        $carnet.val('');
        $name.val('');
    })
        .rut({ formatOn: 'keyup change', minimumLength: minLengthRut, validateOn: 'change' });

    M.Collapsible.init(document.querySelectorAll('.collapsible'));
});