$(document).ready(function () {
    var sidenav = $('#sidenav-details').detach();
    $('#nav-mobile').html('');
    $('#nav-mobile').append(sidenav);
    $('select').formSelect();
    var insuranceList = [];
    $.post('/Patients/Search/InsuranceList', {
        id: $("#DocVM_Id").val(),
        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val()
    }, function (d) {
        function changeInsurances(placeId) {
            var insurances = d[placeId];
            var msg = '';
            for (var i in insurances) {
                msg += "<img alt=\"" + i + "\" src=\"/img/mi/" + i + ".icon.min.png\">";
            }
        }
        var placeId = $('#PlaceId').val();
        changeInsurances(placeId);
        $('#PlaceId').change(function () {
            changeInsurances($(this).val());
        });
    });
    var momentFormat = "DD MMMM YYYY";
    var datepickerFormat = "dd mmmm, yyyy";
    var dateToday = new Date();
    var mañanaTable, tardeTable;
    $('#date-btn').datepicker({
        firstDay: 1,
        format: datepickerFormat,
        autoClose: true,
        minDate: dateToday,
        maxDate: new Date($("#Last").val()),
        container: document.querySelector('body'),
        i18n: {
            months: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
            monthsShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Set", "Oct", "Nov", "Dic"],
            weekdays: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
            weekdaysShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
            weekdaysAbbrev: ["D", "L", "M", "M", "J", "V", "S"],
            cancel: 'Cancelar',
            clear: 'Limpar',
            done: 'Ok'
        },
        showClearBtn: true
    });
    $("#date-view").on('click', 'button.time-select', function () {
        var time = moment($(this).data('booking'), 'H:mm');
        $("#Time").val(moment(time).format('HH:mm:ss'));
        $("#BookForm").submit();
    });
    var noon = moment('12:00', 'HH:mm');
    $("#date-btn").on('change', function () {
        var result = moment($(this).val(), momentFormat).format('YYYY-MM-DD');
        $("#Date").val(result);
        $.ajax({
            url: "/Patients/Booking/BookingMorning",
            data: {
                date: $(this).val(),
                placeId: $("#PlaceId").val(),
                drId: $("#DocVM_Id").val()
            },
            dataType: "json",
            success: function (data) {
                var mañanaData = '';
                var tardeData = '';
                $.each(data, function (index, value) {
                    var bookingData = "<tr>\n                            <td>" + value.hora + "</td>\n                            <td>\n                                <button class=\"time-select btn clinic-desc\" data-booking=\"" + value.hora + "\" class=\"btn waves-effect waves-teal right-align\">SELECCIONAR</button>\n                            </td>\n                            </tr>";
                    var time = moment(value.hora, 'HH:mm');
                    if (time.isBefore(noon)) {
                        mañanaData += bookingData;
                    }
                    else {
                        tardeData += bookingData;
                    }
                });
                mañanaTable = "<table>" + mañanaData + "</table>";
                $('.BookingMorning').html(mañanaTable);
                tardeTable = "<table>" + tardeData + "</table>";
                $('.BookingAfternoon').html(tardeTable);
                $('#map-view').slideToggle();
                $('#date-view').slideToggle();
            }
        });
    });
});
function initDetails() {
    var markers = [];
    var bounds = new google.maps.LatLngBounds();
    var map = new google.maps.Map(document.getElementById('map'), {
        mapTypeControl: false,
        scaleControl: true,
        streetViewControl: false,
        fullscreenControl: false,
        zoomControlOptions: {
            position: google.maps.ControlPosition.RIGHT_CENTER
        }
    });
    var places = {};
    var infowindow = new google.maps.InfoWindow();
    var service = new google.maps.places.PlacesService(map);
    function addEventListener(marker, cid) {
        google.maps.event.addListener(marker, 'click', function () {
            infowindow.setContent(places[cid]);
            infowindow.open(map, marker);
        });
    }
    function addMarkers(place, visibility) {
        if (!(place.id in places)) {
            places[place.id] =
                '<div class="col s12 m6"><div class="card m-0">'
                    + '<div class="card-image">'
                    + ("<img class=\"placeimg\" src=\"" + place.photos[0].getUrl({ 'maxWidth': 200 }) + "\">")
                    + ("<span class=\"card-title\"><b>" + place.name + "</b></span>")
                    + '</div>'
                    + '<div class="card-action">'
                    + ("<p><b>Direcci\u00F3n</b>: " + place.formatted_address + ".<a href=\"" + place.url + "\"><i class=\"material-icons\">location_on</i></a></p>")
                    + '</div></div></div>';
        }
        var match = 0.5;
        var marker = new google.maps.Marker({
            position: place.geometry.location,
            map: map,
            title: place.name + "\n" + place.formatted_address,
            icon: "https://chart.apis.google.com/chart?chst=d_map_spin&chld=1|0|1d999e|16|b|",
            label: place.id,
            opacity: match
        });
        marker.setVisible(visibility);
        addEventListener(marker, place.id);
        markers.push(marker);
        if (visibility)
            map.panTo(marker.getPosition());
    }
    var val = $("#PlaceId > option:selected").val();
    $("#PlaceId > option").each(function (i, e) {
        service.getDetails({
            placeId: $(this).val(),
            fields: ['name', 'formatted_address', 'place_id', 'geometry', 'photo', 'icon', 'address_components']
        }, function (place, _status) {
            bounds.extend(place.geometry.location);
            map.fitBounds(bounds);
            if (map.getZoom() > 18)
                map.setZoom(18);
            addMarkers(place, place.id === val);
            var loc = '';
            $.each(place.address_components, function (i, component) {
                if (component.types[0] == "locality") {
                    loc = component.short_name;
                }
            });
            $(e).text(place.name + ', ' + loc);
            setTimeout(function () { $('select').formSelect(); }, 100);
        });
    });
    $("#PlaceId").on('change', function () {
        markers.forEach(function (marker, i) {
            var selected = marker.getLabel().text === $(this).val();
            infowindow.close();
            marker.setVisible(selected);
            if (selected)
                map.panTo(marker.getPosition());
        });
    });
}
//# sourceMappingURL=doctor-details.js.map