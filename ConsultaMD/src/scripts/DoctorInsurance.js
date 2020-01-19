var $recoverconv = $('#recoverpass');
var $recover = $("#recovery");
var $PassDiv = $('#pass');
$('#Input_Centre').change(function (_) {
    $("#rut").slideToggle();
});
var mi = [
    "https://www.isaprebanmedica.cl/CambiodeClaveBanmedica.aspx",
    "https://www.colmena.cl/web-administracion/sitioprivado/solicitudclave/solicitudclave.jsp",
    "https://clientes.consalud.cl/default.aspx#",
    "https://sitio.cruzblanca.cl/MiCruzBlanca/RecuperacionClave.aspx",
    "https://sv.nuevamasvida.cl/solicitud_clave/?url=recupera",
    "https://www.isaprevidatres.cl/CambioDeClaveVidaTres.aspx"
];
$.validator.addMethod("insurance", function (value, _element, _params) {
    var val = parseInt(value);
    $('.insured, .isapre').slideUp();
    var text = $("#Input_Company_dummy").val();
    $recoverconv.attr("href", mi[val - 2]);
    $recover.attr('src', "/img/mi/" + text + "-icon.min.png");
    $PassDiv.slideDown();
    return true;
});
$.validator.unobtrusive.adapters.add("insurance", [], function (options) {
    options.rules.insurance = {};
    options.messages["insurance"] = options.message;
});
//# sourceMappingURL=DoctorInsurance.js.map