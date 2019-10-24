$(document).ready(function () {
    $('#modal1').modal();
    //$('#Dia').datepicker({
    //    format: 'dd mmmm, yyyy',
    //    i18n: {
    //        months: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
    //        monthsShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Set", "Oct", "Nov", "Dic"],
    //        weekdays: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
    //        weekdaysShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
    //        weekdaysAbbrev: ["D", "L", "M", "M", "J", "V", "S"],
    //        cancel: 'Cancelar',
    //        clear: 'Limpar',
    //        done: 'Ok'
    //    },
    //    onOpen: function () {
    //        this.close();
    //    }
    //});
    $(".paymentType").on('change', function () {
        var type = $(this).val();
        $("#PaymentType").val(type);
    });
});