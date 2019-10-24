function initDetails() {
    var places = {};
    var dt;
    var markers = [];
    var map = new google.maps.Map(document.getElementById('map'), {
        mapTypeControl: false,
        scaleControl: true,
        streetViewControl: false,
        fullscreenControl: false,
        zoom: 18,
        zoomControlOptions: {
            position: google.maps.ControlPosition.RIGHT_CENTER
        }
    });
    var me;
    function moveToLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (pos) {
                var bounds = map.getBounds();
                var post = new google.maps.LatLng(pos.coords.latitude, pos.coords.longitude);
                me = new google.maps.Marker({
                    position: post,
                    map: map,
                    title: "Mi posición actual",
                    icon: "https://chart.apis.google.com/chart?chst=d_map_xpin_icon&chld=pin_star|wc-male|042037|2A4F6E"
                });
                markers.push(me);
                bounds.extend(post);
                map.fitBounds(bounds);
            });
        }
    }
    function fitToMarkers() {
        var bounds = new google.maps.LatLngBounds();
        for (var _a = 0, markers_1 = markers; _a < markers_1.length; _a++) {
            var i = markers_1[_a];
            bounds.extend(i.getPosition());
        }
        map.fitBounds(bounds);
    }
    $('#toggle-center').click(function () {
        var icon = this.firstElementChild;
        if (icon.innerHTML == "location_off") {
            icon.innerHTML = "location_on";
            moveToLocation();
        }
        else {
            icon.innerHTML = "location_off";
            markers.pop();
            me.setMap(null);
            fitToMarkers();
        }
    });
    var infowindow = new google.maps.InfoWindow();
    var service = new google.maps.places.PlacesService(map);
    function addEventListener(marker, placeId) {
        google.maps.event.addListener(marker, 'click', function () {
            infowindow.setContent(places[placeId]);
            infowindow.open(map, marker);
        });
    }
    function addMarkers(place, visibility) {
        if (!(place.place_id in places)) {
            places[place.place_id] =
                '<div class="col s12 m6"><div class="card small">'
                    + '<div class="card-image">'
                    + ("<img class=\"placeimg\" src=\"" + place.photos[0].getUrl({ 'maxWidth': 200 }) + "\">")
                    + ("<span class=\"card-title\"><b>" + place.name + "</b></span>")
                    + '</div>'
                    + '<div class="card-action">'
                    + ("<p><b>Direcci\u00F3n</b>: " + place.formatted_address + ".<a href=\"" + place.url + "\"><i class=\"material-icons\">location_on</i></a></p>")
                    + '</div></div></div>';
        }
        var match = 1;
        var marker = new google.maps.Marker({
            position: place.geometry.location,
            map: map,
            title: place.name + "\n" + place.formatted_address,
            icon: {
                url: "https://chart.apis.google.com/chart?chst=d_map_spin&chld=1|0|1d999e|16|b|",
                labelOrigin: new google.maps.Point(20, 75)
            },
            label: {
                text: place.name,
                fontSize: "1.2rem",
                fontWeight: "800"
            },
            opacity: match,
            place: {
                placeId: place.place_id,
                location: place.geometry.location
            }
        });
        marker.setVisible(visibility);
        addEventListener(marker, place.place_id);
        markers.push(marker);
        if (visibility)
            map.panTo(marker.getPosition());
    }
    function changeInsurances(mdId) {
        var insurances = dt[mdId][0];
        var msg = '<ul>';
        for (var _a = 0, insurances_1 = insurances; _a < insurances_1.length; _a++) {
            var mi = insurances_1[_a];
            msg += "<li><a class=\"btn tooltipped\" data-tooltip=\"" + mi + "\"><img src=\"/img/mi/" + mi + "-icon.min.png\"/></a></li>";
        }
        msg += '</ul>';
        $("#insuranceList").html(msg);
        $('.tooltipped').tooltip();
    }
    var sidenav = $('#sidenav-details').detach();
    $('#nav-mobile').html(sidenav[0]);
    $('select').formSelect();
    var datepickerFormat = "dddd dd mmmm, yyyy";
    var dateToday = new Date();
    var mañanaTable, tardeTable;
    $('#Date').datepicker({
        firstDay: 1,
        format: datepickerFormat,
        autoClose: true,
        minDate: dateToday,
        maxDate: new Date($("#Last").val()),
        disableDayFn: function (d) {
            var mid = $("#MdId").val();
            var result = moment(d, moment.ISO_8601).format("YYYYMMDD");
            return dt[mid][1].indexOf(result) === -1;
        },
        container: document.querySelector('body'),
        i18n: {
            months: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
            monthsShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Set", "Oct", "Nov", "Dic"],
            weekdays: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
            weekdaysShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
            weekdaysAbbrev: ["D", "L", "M", "M", "J", "V", "S"],
            cancel: 'Cancelar',
            clear: 'Limpiar',
            done: 'Ok'
        },
        showClearBtn: true,
        onSelect: function (d) {
            var result = moment(d, moment.ISO_8601).toJSON().replace("Z", "");
            $.post("/Patients/Search/TimeSlots", {
                __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                startDate: result,
                mdId: $("#MdId").val()
            }, function (data) {
                var mañanaData = '';
                var tardeData = '';
                $.each(data, function (i, e) {
                    var bookingData = '<tr>'
                        + ("<td>" + e.startTime + "</td>")
                        + '<td>'
                        + ("<button class=\"time-select btn clinic-desc\" data-id=\"" + e.id + "\" class=\"btn waves-effect waves-teal right-align\">SELECCIONAR</button>")
                        + '</td></tr>';
                    if (~e.startTime.indexOf("a.")) {
                        mañanaData += bookingData;
                    }
                    else {
                        tardeData += bookingData;
                    }
                });
                mañanaTable = "<table>" + mañanaData + "</table>";
                $('#BookingMorning').html(mañanaTable);
                tardeTable = "<table>" + tardeData + "</table>";
                $('#BookingAfternoon').html(tardeTable);
                $('#map-view').slideUp();
                $('#date-view').slideDown();
            });
        }
    });
    $("#date-view").on('click', 'button.time-select', function () {
        $('#TimeSlotId').val($(this).data('id'));
        $("#BookForm").submit();
    });
    $.post('/Patients/Search/MdData', {
        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
        id: $("#DocVM_Id").val()
    }, function (d) {
        dt = d;
        var mdId = $('#MdId').val();
        changeInsurances(mdId);
        $('#MdId').change(function () {
            changeInsurances($(this).val());
        });
    });
    $("#PlaceId > option").each(function (_i, e) {
        var mdId = $('#MdId option:selected').val();
        service.getDetails({
            placeId: $(e).text(),
            fields: ['name', 'formatted_address', 'place_id', 'geometry', 'photo', 'icon', 'address_components']
        }, function (place) {
            addMarkers(place, $(e).val() == mdId);
            setTimeout(function () { $('select').formSelect(); }, 100);
        });
    });
    $("#MdId").on('change', function () {
        $('#map-view').slideDown();
        $('#date-view').slideUp();
        $('#Date').val('Seleccionar Fecha y Hora');
        var val = $(this).val();
        markers.forEach(function (marker) {
            var placeId = $("#PlaceId option[value='" + val + "']").text();
            var selected = marker.get('place').placeId === placeId;
            infowindow.close();
            marker.setVisible(selected);
            if (selected)
                map.panTo(marker.getPosition());
            $('.tooltipped').tooltip();
        });
    });
}
//# sourceMappingURL=doctor-details.js.map