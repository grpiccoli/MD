//Add Error format
$("form input#RUT")
    .rut({ formatOn: 'keyup', validateOn: 'keyup' })
    .on('rutInvalido', function () {
        $(this).parents(".form-group").addClass("has-error");
        $('[data-valmsg-for="RUT"]').html("Rut Inválido");
    })
    .on('rutValido', function () {
        $(this).parents(".form-group").removeClass("has-error");
        $('[data-valmsg-for="RUT"]').html("");
    });

//Fill form with SII data
$('form input#RUT')
    .rut({ formatOn: 'change', validateOn: 'change' })
    .on('rutInvalido', function () {
        $(this).parents(".form-group").addClass("has-error");
        $('[data-valmsg-for="RUT"]').html("Rut Inválido");
        $("input[type=submit]").prop('disabled', true);
    })
    .on('rutValido', function () {
        $(this).parents(".form-group").removeClass("has-error");
        $('[data-valmsg-for="RUT"]').html("");
        if ($('#RUT').val() !== '') {
            $.post('/Home/GetRUTData', { rut: $('#RUT').val(), val: 'name', type: 'rut' },
                function (result) {
                    if (result.error) {
                        $('#Name').val('');
                        $('[data-valmsg-for="RUT"]').html(result.rut + " " + result.error);
                        $("input[type=submit]").prop('disabled', true);
                    } else {
                        $('[data-valmsg-for="RUT"]').empty();
                        $('#Name').val(result.names + " " + result.psur + " " + result.msur);
                        $("input[type=submit]").prop('disabled', false);
                    }
                });
        } else {
            $('#rutVal').empty();
            $('#rutPass').empty();
            $('#Name').val('');
            $("input[type=submit]").prop('disabled', true);
        }
    });

//To Title case function
function toTitleCase(str) {
    return str.replace(/(?:^|\s)\w/g, function (match) {
        return match.toUpperCase();
    });
}
