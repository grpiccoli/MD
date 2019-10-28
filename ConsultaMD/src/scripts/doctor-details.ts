function initDetails() {
    var places: { [cid: string]: string; } = {};
    //define insurancelist
    //type 0 insurance 1 date
    let dt: { [id: number]: { [type: number]: string[] } };
    //ADD MARKER
    let markers: google.maps.Marker[] = [];
    //MAP
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
    var me: google.maps.Marker;
    //Move to new location
    function moveToLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (pos) {
                var bounds = map.getBounds();
                var post = new google.maps.LatLng(pos.coords.latitude, pos.coords.longitude);
                me = new google.maps.Marker({
                    position: post,
                    map: map,
                    title: "Mi posición actual",
                    icon: `https://chart.apis.google.com/chart?chst=d_map_xpin_icon&chld=pin_star|wc-male|042037|2A4F6E`
                });
                markers.push(me);
                bounds.extend(post);
                map.fitBounds(bounds);
            })
        }
    }
    //fit to markers
    function fitToMarkers() {
        //GET BOUNDS
        var bounds = new google.maps.LatLngBounds();
        for (var i of markers) {
            bounds.extend(i.getPosition());
        }
        map.fitBounds(bounds);
    }
    //TOGGLE GEOLOCATION
    $('#toggle-center').click(function () {
        var icon = this.firstElementChild;
        if (icon.innerHTML == "location_off") {
            icon.innerHTML = "location_on";
            moveToLocation();
        } else {
            icon.innerHTML = "location_off";
            markers.pop();
            me.setMap(null);
            fitToMarkers();
        }
    });
    //MAP INFO WINDOW
    let infowindow = new google.maps.InfoWindow();
    //Places Service
    let service = new google.maps.places.PlacesService(map);
    //ADD EVENT LISTENER
    function addEventListener(marker: google.maps.Marker, placeId: string) {
        google.maps.event.addListener(marker, 'click', function () {
            infowindow.setContent(places[placeId]);
            infowindow.open(map, marker);
        });
    }
    //ADD MARKER
    function addMarkers(place: google.maps.places.PlaceResult, visibility: boolean) {
        if (!(place.place_id in places)) {
            places[place.place_id] =
'<div class="col s12 m6"><div class="card small">'
+   '<div class="card-image">'
            + `<img class="placeimg" src="${place.photos[0].getUrl({ 'maxWidth': 200 })}">`
+       `<span class="card-title"><b>${place.name}</b></span>`
+    '</div>'
            + '<div class="card-action">'
            + `<p><b>Dirección</b>: ${place.formatted_address}.<a href="${place.url}"><i class="material-icons">location_on</i></a></p>`
+   '</div></div></div>';
        }
        var match = 1;
        var marker = new google.maps.Marker({
            position: place.geometry.location,
            map: map,
            title: `${place.name}\n${place.formatted_address}`,
            icon: {
                url: `https://chart.apis.google.com/chart?chst=d_map_spin&chld=1|0|1d999e|16|b|`,
                labelOrigin: new google.maps.Point(20,75)
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
        if (visibility) map.panTo(marker.getPosition());
    }
    //change insurance list function
    function changeInsurances(mdId: number) {
        let insurances = dt[mdId][0];
        let msg = '<ul>';
        for (var mi of insurances) {
            msg += `<li><a class="btn tooltipped" data-tooltip="${mi}"><img src="/img/mi/${mi}-icon.min.png"/></a></li>`
        }
        msg += '</ul>';
        $("#insuranceList").html(msg);
        M.Tooltip.init(document.querySelectorAll('.tooltipped'));
    }
    let sidenav = $('#sidenav-details').detach();
    $('#nav-mobile').html(sidenav[0]);
    M.FormSelect.init(document.querySelectorAll('select'));
    var datepickerFormat = "dddd dd mmmm, yyyy";
    var dateToday = new Date();
    let mañanaTable, tardeTable: string;
    M.Datepicker.init(document.getElementById('Date'), {
        firstDay: 1,
        format: datepickerFormat,
        autoClose: true,
        minDate: dateToday,
        maxDate: new Date($("#Last").val() as string),
        disableDayFn: function (d) {
            var mid = $("#MdId").val();
            var result = moment(d, moment.ISO_8601).format("YYYYMMDD");
            return dt[mid as number][1].indexOf(result) === -1;
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
            }, function (data: TimeSlotVM[]) {
                let mañanaData = '';
                let tardeData = '';
                $.each(data, (i, e) => {
                    var bookingData = '<tr>'
                        + `<td>${e.startTime}</td>`
                        + '<td>'
                        + `<button class="time-select btn clinic-desc" data-id="${e.id}" class="btn waves-effect waves-teal right-align">SELECCIONAR</button>`
                        + '</td></tr>';
                    if (~e.startTime.toString().indexOf("a.")) {
                        mañanaData += bookingData;
                    } else {
                        tardeData += bookingData;
                    }
                });
                mañanaTable = `<table>${mañanaData}</table>`;
                $('#BookingMorning').html(mañanaTable);
                tardeTable = `<table>${tardeData}</table>`;
                $('#BookingAfternoon').html(tardeTable);
                $('#map-view').slideUp();
                $('#date-view').slideDown();
            }
            );
        }
    });
    //$('#Date').datepicker({
    //    firstDay: 1,
    //    format: datepickerFormat,
    //    autoClose: true,
    //    minDate: dateToday,
    //    maxDate: new Date($("#Last").val() as string),
    //    disableDayFn: function (d) {
    //        var mid = $("#MdId").val();
    //        var result = moment(d, moment.ISO_8601).format("YYYYMMDD");
    //        return dt[mid as number][1].indexOf(result) === -1;
    //    },
    //    container: document.querySelector('body'),
    //    i18n: {
    //        months: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
    //        monthsShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Set", "Oct", "Nov", "Dic"],
    //        weekdays: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"],
    //        weekdaysShort: ["Dom", "Lun", "Mar", "Mie", "Jue", "Vie", "Sab"],
    //        weekdaysAbbrev: ["D", "L", "M", "M", "J", "V", "S"],
    //        cancel: 'Cancelar',
    //        clear: 'Limpiar',
    //        done: 'Ok'
    //    },
    //    showClearBtn: true,
    //    onSelect: function (d) {
    //        var result = moment(d, moment.ISO_8601).toJSON().replace("Z", "");
    //        $.post("/Patients/Search/TimeSlots", {
    //            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
    //            startDate: result,
    //            mdId: $("#MdId").val()
    //        }, function (data: TimeSlotVM[]) {
    //            let mañanaData = '';
    //            let tardeData = '';
    //            $.each(data, (i, e) => {
    //                var bookingData = '<tr>'
    //                    + `<td>${e.startTime}</td>`
    //                    + '<td>'
    //                    + `<button class="time-select btn clinic-desc" data-id="${e.id}" class="btn waves-effect waves-teal right-align">SELECCIONAR</button>`
    //                    + '</td></tr>';
    //                if (~e.startTime.indexOf("a.")) {
    //                    mañanaData += bookingData;
    //                } else {
    //                    tardeData += bookingData;
    //                }
    //            });
    //            mañanaTable = `<table>${mañanaData}</table>`;
    //            $('#BookingMorning').html(mañanaTable);
    //            tardeTable = `<table>${tardeData}</table>`;
    //            $('#BookingAfternoon').html(tardeTable);
    //            $('#map-view').slideUp();
    //            $('#date-view').slideDown();
    //        }
    //        );
    //    }
    //});
    $("#date-view").on('click', 'button.time-select', function () {
        $('#TimeSlotId').val($(this).data('id'));
        $("#BookForm").submit();
    });
    //get insurance list
    $.post('/Patients/Search/MdData',
        {
            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
            id: $("#DocVM_Id").val()
            //type 0 insurance 1 dates
        }, function (d: { [id: number]: { [type: number]: string[] } }) {
            dt = d;
            let mdId = $('#MdId').val() as number;
            changeInsurances(mdId);
            $('#MdId').change(function () {
                changeInsurances($(this).val() as number);
            });
        });
    //GET ALL Places
    $("#PlaceId > option").each(function (_i, e) {
        var mdId = $('#MdId option:selected').val();
        service.getDetails(
            {
                placeId: $(e).text() as string,
                fields: ['name', 'formatted_address', 'place_id', 'geometry', 'photo', 'icon', 'address_components']
            },
            function (place) {
                addMarkers(place, $(e).val() == mdId);
                setTimeout(function () { $('select').formSelect() }, 100);
            });
    });
    //On MediumDoctor change
    $("#MdId").on('change', function () {
        $('#map-view').slideDown();
        $('#date-view').slideUp();
        $('#Date').val('Seleccionar Fecha y Hora');
        var val = $(this).val();
        markers.forEach(function (marker) {
            var placeId = $(`#PlaceId option[value='${val}']`).text();
            var selected = marker.get('place').placeId === placeId;
            infowindow.close();
            marker.setVisible(selected);
            if (selected) map.panTo(marker.getPosition());
            M.Tooltip.init(document.querySelectorAll('.tooltipped'));
        });
    });
}