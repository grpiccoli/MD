//DATEPICKER
var dateToday = new Date();
var yr = dateToday.getFullYear();
var datepickerFormat = "dddd dd mmmm, yyyy";
let mañanaTable, tardeTable: string;

function min(array: number[]) {
    return Math.min.apply(Math, array);
};

function isNullOrWhitespace(input: string): boolean {
    if (typeof input === 'undefined' || input == null) return true;
    return input.replace(/\s/g, '').length < 1;
}
$("#date-view").on('click', 'button.time-select', function () {
    $.post("./Reservation", {
        id: $(this).data('id'),
        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
    });
});
//Open dates
function agendar2(mdId: string) {
    let minDate = $("#MinDate").val();
    let last = $("#Last").val();
    let lastYr = moment(last).toDate().getFullYear();
    if (isNullOrWhitespace(minDate as string)) {
        minDate = moment(dateToday).toJSON().replace("Z", "");
    } else {
        minDate = moment(minDate).toJSON().replace("Z", "");
    }
    let maxDate = $("#MaxDate").val();
    if (isNullOrWhitespace(maxDate as string)) {
        maxDate = moment(last).toJSON().replace("Z", "");
    } else {
        maxDate = moment(maxDate).toJSON().replace("Z", "");
    }
    console.log(mdId);
    $.post("/Patients/Search/GetDates", {
        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
        mdId: mdId,
        minDate: minDate,
        maxDate: maxDate,
    }, (dt: number[]) => {
            console.log(mdId);
            var datePicker = M.Datepicker.init(document.getElementById('Date'), {
                firstDay: 1,
                autoClose: true,
                minDate: dateToday,
                showDaysInNextAndPreviousMonths: true,
                maxDate: new Date($("#Last").val() as string),
                defaultDate: moment(min(dt), "YYYYMMDD").toDate(),
                yearRange: [yr, lastYr],
                setDefaultDate: true,
                disableDayFn: (d) => {
                    var result = parseInt(moment(d, moment.ISO_8601).format("YYYYMMDD"));
                    return dt.indexOf(result) === -1;
                },
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
                onSelect: function (d) {
                    console.log(mdId);
                    var result = moment(d, moment.ISO_8601).toJSON().replace("Z", "");
                    $.post("/Patients/Search/TimeSlots", {
                        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                        startDate: result,
                        mdId: mdId
                    }, function (data: TimeSlotVM[]) {
                        let mañanaData = '';
                        let tardeData = '';
                        $.each(data, (i, e) => {
                            var bookingData = '<tr>'
                                + `<td>${e.startTime}</td>`
                                + '<td>'
                                + `<button class="time-select btn clinic-desc" data-id="${e.id}" class="btn waves-effect waves-teal right-align">SELECCIONAR</button>`
                                + '</td></tr>';
                            if (~e.startTime.indexOf("a.")) {
                                mañanaData += bookingData;
                            } else {
                                tardeData += bookingData;
                            }
                        });
                        mañanaTable = `<table>${mañanaData}</table>`;
                        $('#BookingMorning').html(mañanaTable);
                        tardeTable = `<table>${tardeData}</table>`;
                            $('#BookingAfternoon').html(tardeTable);
                            M.Sidenav.getInstance(document.getElementById('slide-action')).close();
                        $('#map-view').slideUp();
                        $('#list-view').slideUp();
                        $('#date-view').slideDown();
                            $('#toggle-view').html("volver al mapa");
                    }
                    );
                }
            });
            datePicker.open();
            return false;
    });
}
//REDIRECT TO DOCTOR DETAILS
function agendar(run: number, mdId: string) {
    var q = $("#Insurance, #Ubicacion, #MinTime, #MaxTime")
        .serialize();
    var date = $("#MinDate,#MaxDate").serializeArray();
    var array: string[] = [q];
    array.push(`MdId=${mdId}`);
    array.push(`Last=${$('#Last').val()}`);
    date.forEach((value) => {
        if (value.value === '') return;
        array.push(value.name + '=' + moment(value.value).toJSON().replace("Z", ""));
    });
    var days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    days.forEach((value) => {
        var prop = $(`#${value}`).prop('checked');
        if (typeof (prop) !== 'undefined') {
            array.push(value + "=" + prop);
        }
    });
    window.location.href = `/Patients/Search/DoctorDetails/${run}?` + array.join("&");
    return false;
}
//INIT MAP AND GET DOCTOR LIST
function initMap() {
    //INSURANCES
    let mis: Array<string> = [
        'Particular',
        'FONASA',
        'Banmédica',
        'Colmena',
        'Consalud',
        'CruzBlanca',
        'Nueva Masvida',
        'Vida Tres'
    ];
    //ADD MARKER
    let markers: google.maps.Marker[] = [];
    let markerCluster: MarkerClusterer;
    //fit to markers
    function fitToMarkers() {
        //GET BOUNDS
        var bounds = new google.maps.LatLngBounds();
        for (var i of markers) {
            bounds.extend(i.getPosition());
        }
        map.fitBounds(bounds);
    }
    //MAP
    var map = new google.maps.Map(document.getElementById('map'), {
        mapTypeControl: false,
        scaleControl: true,
        streetViewControl: false,
        fullscreenControl: false,
        gestureHandling: 'greedy',
        zoomControlOptions: {
            position: google.maps.ControlPosition.RIGHT_CENTER
        }
    });
    var places: { [cid: string]: { [type: string]: string; } } = {};
    //destroy and create tabs in sidenav
    var slide_action = M.Sidenav.init(document.getElementById('slide-action'), {
        draggable: false,
        onCloseEnd: _ => {
            [].forEach.call(document.querySelectorAll('#slide-action .tabs'), function (tab: Element) {
                M.Tabs.getInstance(tab).destroy();
            });
            //$('#slide-action .tabs').tabs('destroy');
            [].forEach.call(document.querySelectorAll('#slide-action .tooltipped'), function (tooltip: Element) {
                M.Tooltip.getInstance(tooltip).destroy();
            });
            //$('#slide-action .tooltipped').tooltip('destroy');
        }
    });
    //document.querySelectorAll("#slide-action ul").forEach((e) => {
    //    e.addEventListener('dragstart', function () {
    //        slide_action.options.draggable = false;
    //    });
    //    e.addEventListener('dragend', function () {
    //        slide_action.options.draggable = true;
    //    });
    //});
    //ADD EVENT LISTENER
    function addEventListener(marker: google.maps.Marker, cid: string) {
        google.maps.event.addListener(marker, 'click', _ => {
            $("#slide-header").html(places[cid]["header"]);
            $("#slide-content").html(places[cid]["content"]);
            setTimeout(() => {
                slide_action.open();
                M.Tabs.init(document.querySelectorAll('#slide-action .tabs'));
                M.Tooltip.init(document.querySelectorAll('#slide-action .tooltipped'));
            }, 500);
        });
    }
    // Sets the map on all markers in the array.
    function setMapOnAll(map: google.maps.Map) {
        for (var i of markers) {
            i.setMap(map);
        }
    }
    // Removes the markers from the map, but keeps them in the array.
    function clearMarkers() {
        setMapOnAll(null);
    }
    // Deletes all markers in the array by removing references to them.
    function deleteMarkers() {
        clearMarkers();
        markers = [];
    }
    function format(n: number) {
        return n.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');
    }
    function makeCard(item: ResultVM, place: Place) {
        var esp = `<div id="${item.cardId}sp" class="tab-content esp">`, title = '', convenios = `<div id="${item.cardId}insr" class="tab-content mis">`;
        var conv_tab = `<li class="tab"><a class="mis" href="#${item.cardId}insr">Convenios</a></li>`;
        var esp_tab = `<li class="tab"><a class="esp" href="#${item.cardId}sp">Especialidad</a></li>`;
        if (item.insurances.length !== 0) {
            var plural = item.insurances.length > 1 ? 's' : '';
            convenios += `<label>Convenio${plural}:</label><ul>`;
            for (var i of item.insurances) {
                var mi = mis[i];
                convenios += `<li><a class="btn tooltipped" data-tooltip="${mi}"><img src="/img/mi/${mi}-icon.min.png"/></a></li>`;
            }
            convenios += "</ul>";
        }
        convenios += '</div>';
        if (item.especialidad !== null) {
            title = `Dr${item.sex ? "" : "a"}.`;
            esp += `<label>Esp.</label><span>${item.especialidad}</span>`;
        }
        esp += '</div>';
        var office = `<p class="address">${place.address}`;
        office += isNullOrWhitespace(item.office) ? '</li>' : ` <span>${item.office}</span></p>`;
        return '<div class="col s12 m6 l4">'
            + '<div class="card horizontal sticky-action">'
            +   '<div class="card-image">'
            +       `<img src="/img/doc/${item.run}.min.jpg"/>`
            +   '</div>'
            +   '<div class="card-stacked">'
            +       '<div class="card-content">'
            +           `<span class="card-title activator">${title} ${item.dr}</span>`
            +       '</div>'
            +       '<div class="card-tabs">'
            +       '<ul class="tabs tabs-fixed-width">'
            + `<li class="tab"><a class="time" href="#${item.cardId}next">Prox. Hora</a></li>`
            + `<li class="tab"><a class="addr" href="#${item.cardId}addr">Dirección</a></li>`
            + `<li class="tab"><a class="price" href="#${item.cardId}prce">Valor Particular</a></li>`
            + conv_tab
            + esp_tab
            +       '</ul>'
            +       '</div>'
            +       '<div class="card-content grey lighten-4">'
            + `<div id="${item.cardId}next" class="tab-content time">${item.hora}</div>`
            + `<div id="${item.cardId}addr" class="tab-content addr">${office}</div>`
            + `<div id="${item.cardId}prce" class="tab-content price"><span><i class="material-icons left">attach_money</i>${format(item.price)}</span></div>`
            + convenios
            + esp
            +       '</div>'
            + '<div class="card-action center">'
            + `<a href="#" onclick="agendar2(${item.cardId})">Agendar</a>`
            +       '</div>'
            +   '</div>'
            +   '<div class="card-reveal">'
            +       '<span class="card-title">'
            +           `<i class="material-icons right">close</i>${title} ${item.dr}`
            +       '</span>'
            +       convenios
            +       '<label>Valor Particular:</label>'
            +       `<span>$${format(item.price)}</span>`
            +   '</div>'
            + '</div></div>';
    }
    //NAME FILTER
    let dt: { [name: string]: string };
    //Add Marker
    function addMarkers(value: ResultsVM) {
        if (!(value.place.cId in places)) {
            places[value.place.cId] = {};
            //MARKER SIDE PANEL
            places[value.place.cId]["header"] =
'<div class="col s12 m6">'
+   '<div class="card m-0">'
+       '<div class="card-image">'
+           `<img class="activator placeimg" src="https://maps.googleapis.com/maps/api/place/photo?maxwidth=300&photoreference=${value.place.photoId}&key=AIzaSyDkCLRdkB6VyOXs-Uz_MFJ8Ym9Ji1Xp3rA">`
+           `<span class="card-title activator"><b>${value.place.name}</b></span>`
+       '</div>'
+       '<div class="card-content">'
+           '<span class="card-title activator grey-text text-darken-4">Ver detalles<i class="material-icons right">more_vert</i></span>'
+       '</div>'
+       '<div class="card-reveal">'
+           `<span class="card-title grey-text text-darken-4">${value.place.name}<i class="material-icons right">close</i></span>`
+           `<p><b>Dirección</b>: ${value.place.address}.<a href="https://maps.google.com/?cid=${value.place.cId}"><i class="material-icons">location_on</i></a></p>`
+       '</div>'
+   '</div></div></div>';
            places[value.place.cId]["list"] =
                `<div class="col s12"><h5>${value.place.name}</h5><p>${value.place.address}</p><hr/></div><div id="${value.place.cId}" class="list-place"></div>`
        }
        //LIST
        $("#list").append(places[value.place.cId]["list"]);
        var cnt = 0;
        var match = 0.5;
        var content: string = '';
        $.each(value.items, function () {
            if(!(this.dr in dt)) dt[this.dr] = `/img/doc/${this.run}.min.jpg`;
            var card = makeCard(this, value.place)
            content += card;
            cnt++;
            if (match === 0.5 && this.match) match = 1;
        });
        places[value.place.cId]["content"] = content;
        var colorMrk = "1d999e";
        var sizeMkr = "16";
        let marker = new google.maps.Marker({
            position: new google.maps.LatLng({ lat: value.place.latitude, lng: value.place.longitude }),
            map: map,
            title: `${value.place.name}\n${value.place.address}`,
            icon: `https://chart.apis.google.com/chart?chst=d_map_spin&chld=1|0|${colorMrk}|${sizeMkr}|b|${cnt}`,
            zIndex: cnt,
            opacity: match
        });
        markers.push(marker);
        addEventListener(marker, value.place.cId);
    }
    //PROCESS CALLBACK DATA
    function getList(data: Array<ResultsVM>) {
        var count = data.length;
        if (count === 0) {
            loaderStop();
            $('button').removeAttr('disabled');
            alert("No hay resultados para los filtros seleccionados");
            if (typeof (markerCluster) !== 'undefined') {
                markerCluster.clearMarkers();
            }
        }
        $("#list").empty();
        dt = {};
        $.each(data,
            function () {
                addMarkers(this);
                if (!--count) {
                    fitToMarkers();
                    loaderStop();
                    var search = document.getElementById('search-filter');
                    M.Autocomplete.init(search, {
                        data: dt,
                        limit: 10,
                        onAutocomplete: (val) => {
                            var run = parseInt(dt[val].replace(/.*\//g, '').replace(/\..*/g, ''));
                            agendar(run, null);
                        }
                    });
                    $('#close-search').click(_ => {
                        $(search).val('');
                    });
                    if (typeof(markerCluster) !== 'undefined') {
                        markerCluster.clearMarkers();
                    }
                    //MARKER CLUSTERER
                    var mcOptions = {
                        gridSize: 40,
                        imagePath: '/img/cluster/m'
                    };
                    markerCluster = new MarkerClusterer(map, markers, mcOptions);
                    markerCluster.setCalculator((markers: google.maps.Marker[], numStyles: number) => {
                        var index = 1;
                        var count = 0;
                        for (var i of markers) {
                            count += i.getZIndex();
                        }
                        var index = Math.round(Math.log(count / 4));
                        if (index > 5) index = 5;
                        return {
                            text: count,
                            index: index
                        };
                    });
                    M.Collapsible.init(document.querySelectorAll('.collapsible'));
                    $('button').removeAttr('disabled');
                }
            });
    }
    //SEARCH FUNCTIONS
    function getAllquerys() {
        var array = $("#filter input[name='__RequestVerificationToken'],"
            + "#Insurance, #Ubicacion, #Especialidad, #Sex, #HighlightInsurance, #MinTime, #MaxTime")
            .serializeArray();
        var date = $("#MinDate,#MaxDate").serializeArray();
        date.forEach((value) => {
            if (value.value === '') return;
            array.push({
                name: value.name,
                value: moment(value.value).toJSON().replace("Z", "")
            });
        });
        var days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
        days.forEach((value) => {
            array.push({
                name: value,
                value: $(`#${value}`).prop('checked')
            })
        });
        return array;
    }
    //GET DATA
    function getData() {
        loaderStart();
        $('button').prop('disabled', true);
        $.post("/Patients/Search/MapList", getAllquerys(), getList);
    }
    getData();

    var me: google.maps.Marker;
    //Move to new location
    function moveToLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition((pos) => {
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
    //TOGGLE LIST/MAP
    $('#toggle-view').click(function () {
        $("#date-view").slideUp();
        if ($("#map-view").css("display") === "block") {
            $(this).html("volver al mapa");
            $.each(places, function (i) {
                $(`#${i}`).html(this["content"])
            });
            M.Tooltip.init(document.querySelectorAll('#list .tooltipped'));
            //$('#list .tooltipped').tooltip();
            $("#map-view").slideUp();
            $("#list-view").slideDown();
            M.Tabs.init(document.querySelectorAll('#list .tabs'));
            //$('#list .tabs').tabs();
        } else {
            $("#map-view").slideDown();
            $("#list-view").slideUp();
            $(this).html("ver como lista");
            setTimeout(fitToMarkers, 400);
            setTimeout(() => {
                [].forEach.call(document.querySelectorAll('#list .tabs'), function (tab: Element) {
                    M.Tabs.getInstance(tab).destroy();
                });
                [].forEach.call(document.querySelectorAll('#list .tooltipped'), function (tooltip: Element) {
                    M.Tooltip.getInstance(tooltip).destroy();
                });
                //$('#list .tabs').tabs('destroy');
                //$('#list .tooltipped').tooltip('destroy');
                $('#list .list-place').html('');
            }, 200);
        }
    });
    var change = false;
    function changeFilter() {
        if (change) {
            deleteMarkers();
            getData();
            change = false;
        }
    }
    function filterBtnTxt(el: any) {
        var id = $(el.target).attr('id');
        var text = id;
        var data = $(el.target).select2('data');
        if (data.length > 0) {
            var number = data.length > 1 ? ` +${data.length - 1}` : '';
            text = `${data[0].text.substring(0, 8)}&hellip;${number}`;
        }
        $(`#btn${id}`).html(text);
        change = true;
    }
    //FILTER TIMES
    function filterTimes(value: number, _t: number) {
        if (value % 6) {
            return 0;
        } else {
            return 1;
        }
    }
    //TIME SLIDER
    noUiSlider.create(document.getElementById('time-slider'), {
        start: [0, 24],
        connect: true,
        step: 1,
        orientation: 'horizontal',
        margin: 6,
        range: {
            'min': 0,
            'max': 24
        },
        format: {
            to: (value: number) => {
                return Math.ceil(value);
            },
            from: (value: string) => {
                return Number(value.replace(/\D/g, ''));
            }
        },
        pips: {
            mode: 'steps',
            density: 4,
            filter: filterTimes,
            format: {
                to: (value: number) => {
                    var suffix = "AM";
                    if (value >= 12) {
                        if (value !== 24) {
                            suffix = "PM";
                        }
                        if (value !== 12) {
                            value -= 12;
                        }
                    }
                    return `${Math.ceil(value)} ${suffix}`;
                },
                from: (value: string) => {
                    return Number(value.replace(/\D/g, ''));
                }
            }
        }
    }).on('set', (values: any, handle: any) => {
        if (handle) {
            $('#MaxTime').val(values[handle] === 24 ? '' : values[handle]);
        } else {
            $('#MinTime').val(values[handle] === 0 ? '' : values[handle]);
        }
    });
    M.Datepicker.init(document.querySelectorAll('.datepicker'), {
        firstDay: 1,
        format: datepickerFormat,
        autoClose: true,
        minDate: dateToday,
        maxDate: new Date($("#Last").val() as string),
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
        showClearBtn: true
    });
    //$('.datepicker').datepicker();
    $('#MinDate').change(function (d) {
        var max = M.Datepicker.getInstance(document.getElementById('MaxDate'));
        max.options.minDate = $(this).val() === '' ?
            dateToday :
            moment($(this).val()).toDate();
    });
    //FILTERS MODAL
    var filters = M.Modal.init(document.getElementById('filters-pane'), {
        onCloseStart: changeFilter
    });
    //let filters = $('#filters-pane').modal({
    //    onCloseStart: changeFilter
    //});
    $('#filter-action').click(event => {
        event.preventDefault();
        filters.close();
            //.modal('close');
    });
    $('#filter-clear').click(event => {
        event.preventDefault();
        //console.log(location, window.location);
        window.location.reload();
    });
    $('#filter input').change(_ => {
        change = true;
    });
    $('.fltbtn').click((panel: any) => {
        var id = $(panel.target).attr('id');
        switch (id) {
            case "btnEspecialidad":
                $('.fltcol').hide();
                filters.open();
                //filters.modal('open');
                $('#especialities-col').fadeIn();
                $('#especialities-col input.select2-search__field').trigger('keyup');
                break;
            case "btnUbicacion":
                $('.fltcol').hide();
                filters.open();
                //filters.modal('open');
                $('#locations-col').fadeIn();
                $('#locations-col input.select2-search__field').trigger('keyup');
                break;
            case "btnFechaHora":
                $('.fltcol').hide();
                filters.open();
                //filters.modal('open');
                $('#dates-col').fadeIn();
                $('#dates-col input.select2-search__field').trigger('keyup');
                break;
            case "all1":
            case "all2":
                filters.open();
                //filters.modal('open');
                $('.fltcol').fadeIn();
                $('#filters-pane input.select2-search__field').trigger('keyup');
                break;
        }
    });
    //select show list
    var show = M.FormSelect.init(document.getElementById('show'));
    show.el.addEventListener("change", (e) => {
        $(`li.tab a`).removeClass("active");
        var s = show.el as HTMLSelectElement;
        var val = s.options[s.selectedIndex].value;
        var $el = $(`li.tab a.${val}`).addClass("active");
        var pos = 0;
        if (val === 'esp' || val === 'mis') pos += 200;
        $el.parent().parent().animate({ scrollLeft: pos });
        M.Tabs.init(document.querySelectorAll('.tabs'));
        //$('.tabs').tabs();
    });
    //initialize selects
    $.post('/Patients/Search/FilterLists', $("#filter input[name='__RequestVerificationToken']").serializeArray(), (d) => {
        //SPECIALTY SELECT
        $('#Especialidad').select2({
            theme: "material",
            placeholder: "Seleccione especialidades",
            data: d.esp
        }).change(filterBtnTxt);
        //LOCATION SELECT
        $('#Ubicacion').select2({
            theme: "material",
            placeholder: "Seleccione ubicaciones",
            data: d.loc
        }).change(filterBtnTxt);
        //SEX SELECT
        $('#Sex').select2({
            theme: "material",
            maximumSelectionLength: 1,
            placeholder: "Seleccione sexo del profesional médico"
        });
        //NORMALIZE SELECT2 INPUT
        $("input.select2-search__field")
            .addClass("browser-default");
    });
}