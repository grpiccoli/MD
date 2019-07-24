$(function () {
    function toTitleCase(str) {
        return str.replace(/(?:^|\s)\w/g, function (match) {
            return match.toUpperCase();
        });
    }
    $('#modal-action').on('show.bs.modal', function (_e) {
        //var button = $(event.relatedTarget); // Button that triggered the modal
        //var url = button.attr("href");
        //var url = event.url;
        $('#loading').show();
        var modal = $(this);
        var url = $("#event").data("url");
        // note that this will replace the content of modal-content everytime the modal is opened
        var data = {
            "Id": $("#event").attr("data-id"),
            "DoctorId": $("#event").attr("data-doctorid"),
            "Start": $("#event").attr("data-start"),
            "End": $("#event").attr("data-end"),
            "Atencion": $("#event").attr("data-atencion"),
            "Lugar": $("#event").attr("data-lugar"),
            "RUN": $("#event").attr("data-run"),
            "LocId": $("#event").attr("data-locid"),
            "Prevision": $("#event").attr("data-prevision"),
            "__RequestVerificationToken": $("#doctor input[name = '__RequestVerificationToken']").val()
        };
        modal.find('.modal-content').load(url, data, function loadListen() {
            //$(".flatpickr").flatpickr({"locale": "es", "allowInput": true});
            $('.selectpicker').selectpicker();

            $(".phone-format")
                .keyup(function () {
                    var val_old = $(this).val().replace(/\D/g, '');
                    if (val_old.trim().length > 1) {
                        var phoneNumber = libphonenumber.parsePhoneNumberFromString(val_old, 'CL');
                        if (phoneNumber.isValid()) {
                            $(this).focus().val('').val(phoneNumber.formatNational());
                            $(this).removeClass("is-invalid");
                            $(this).addClass("is-valid");
                            $(this).parent().nextAll("span:first").empty();
                            var botones = $(this).prev().find("button");
                            if ($(this).val().toString().startsWith('9')) {
                                botones.addClass('btn-outline-success');
                                botones.prop('disabled', false);
                            } else {
                                $(botones[0]).addClass('btn-outline-success');
                                $(botones[0]).prop('disabled', false);
                            }
                            if ($(".phone-format.is-invalid").length === 0) {
                                $('#btn-add-phone').prop('disabled', false);
                                $("button[type=submit]").prop('disabled', false);
                            }
                        } else {
                            $(this).removeClass("is-valid");
                            botones = $(this).prev().find("button");
                            botones.prop('disabled', true);
                            botones.removeClass('btn-outline-success');
                            if (phoneNumber.number.length < 12) {
                                $(this).focus().val('').val(new libphonenumber.AsYouType('CL').input(val_old));
                            } else {
                                $(this).addClass("is-invalid");
                                $(this).parent().nextAll("span:first").html("teléfono no válido");
                                $('#btn-add-phone').prop('disabled', true);
                                $("button[type=submit]").prop('disabled', true);
                            }
                        }
                    }
                })
                .trigger('keyup');

            $("#RUT")
                .rut({ formatOn: 'keyup', validateOn: 'keyup' })
                .on('rutInvalido', function () {
                    $(this).removeClass("is-valid");
                    $("button[type=submit]").prop('disabled', true);
                    if ($(this).val().trim().length !== 0) {
                        $(this).addClass("is-invalid");
                        $('[data-valmsg-for="RUT"]').html("Rut Inválido");
                    } else {
                        $("#loading").hide();
                        $('#Name').val(null);
                    }
                })
                .on('rutValido', function () {
                    $(this).removeClass("is-invalid");
                    $(this).addClass("is-valid");
                    $('[data-valmsg-for="RUT"]').empty();
                    $.getJSON('https://siichile.herokuapp.com/consulta', { rut: $('#RUT').val().replace(/[^0-9Kk]/g, '') },
                        function (result) {
                            if (result.error) {
                                $('#Name').val(null);
                                $('[data-valmsg-for="RUT"]').html(result.rut + " " + result.error);
                                $("button[type=submit]").prop('disabled', true);
                            } else {
                                $('#loading').show();
                                $('[data-valmsg-for="RUT"]').empty();
                                $('#Name').val(toTitleCase(result.razon_social.toString().toLowerCase()));
                                $.post('/Home/GetData', { "rut": $("#RUT").val().replace(/\./g, ""), "today": $("#Start").val() },
                                    function (data) {
                                        if (data.nameP !== null) {
                                            if (data.idtoday !== null) {
                                                if (data.idtoday !== $("#Id").val())
                                                    alert('este RUT ya tiene una cita para hoy');
                                            } else {
                                                if (data.code !== "" && data.phone !== "") {
                                                    $("#Phone1").val(data.code + data.phone);
                                                    $("#Phone1").trigger('keyup');
                                                    if (data.cellphone !== null) {
                                                        $('#btn-add-phone').trigger('click');
                                                        $("#Phone2").val(data.cellphone);
                                                        $("#Phone2").trigger('keyup');
                                                    }
                                                    if (data.cellphoneAdditional !== null) {
                                                        $('#btn-add-phone').trigger('click');
                                                        $("#Phones3").val(data.cellphoneAdditional);
                                                        $("#Phone3").trigger('keyup');
                                                    }
                                                }
                                                if (data.codeHealthcare !== null)
                                                    $('#Prevision').val(data.codeHealthcare);
                                                $('.selectpicker').selectpicker('refresh');
                                                if (data.day !== null && data.month !== null && data.year !== null) {
                                                    $("#Birth").val(data.year + '-' + data.month + '-' + data.day);
                                                }
                                            }
                                        }
                                        $('#loading').hide();
                                    });
                                if ($(".is-invalid").length === 0) {
                                    $("button[type=submit]").prop('disabled', false);
                                }
                            }
                        });
                })
                .trigger("keyup");

            $('.btn-remove-phone').click(function () {
                $(this).closest('.form-group').addClass('d-none');
                var parent = $(this).parent().parent();
                parent.find('.phone-format').val(null);
                parent.nextAll("span:first").empty();
                var a = $(this).parent().prev().prev().find("a");
                a.attr("href", "");
                a.removeClass("btn-outline-info");
                a.addClass("disabled");
                $('#grp-add-phone').removeClass('d-none');
            });
            $('#btn-add-phone').click(function () {
                var index = $('.phone-group').length - $('.phone-group.d-none').length + 1;
                $('#phone' + index + '-group').removeClass('d-none');
                if (index === 3) $('#grp-add-phone').addClass('d-none');
            });
            $('#btn-submit').click(function () {
                $('#loading').show();
                var form = $(this).parents('.modal').find('form');
                modal.find('.modal-content').load(form.attr('action'), form.serializeArray(), function () {
                    if ($('#valid').val() === 'True') {
                        $('.modal').modal('hide');
                        cal.refetchEvents();
                    } else {
                        alert($('#Message').val());
                        loadListen();
                    }
                    $(".tooltip").css("visibility", "hidden");
                    $('#loading').hide();
                });
            });
            $('#btn-delete').click(function () {
                $('#Delete').prop("checked", true);
                $('#btn-submit').trigger('click');
            });

            //$('#loading').hide();
        });
    });
    // when the modal is closed
    $('#modal-action').on('hidden.bs.modal', function () {
        // remove the bs.modal data attribute from it
        $(this).removeData('bs.modal');
        // and empty the modal-content element
        $('#modal-action .modal-content').empty();
    });
    $('#modal-action').change(function () {
        $.validator.unobtrusive.parse('form#modal-form');
    });
});