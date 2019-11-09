//DATEPICKER
const dateToday = new Date();
const yr = dateToday.getFullYear();
let mañanaTable, tardeTable: string;
const ASPNETDateFormat = 'YYYY-MM-DDTHH:mm:ss.000';
//test if string is null or whitespace
function isNullOrWhitespace(input: string): boolean {
    if (typeof input === 'undefined' || input == null) return true;
    return input.replace(/\s/g, '').length < 1;
}
function min(array: number[]) {
    return Math.min.apply(Math, array);
}
function getContent(mdId: number) {
    var card = $(places[mdId]['card']);
    var string = $(card.find('.card-title')[0])
        .removeClass('activator').attr('onclick', null)
        .append('<i class="material-icons right">close</i>')[0].outerHTML;
    var cardtabs = card.find('.tab > a');
    var content = card.find('.tab-content');
    for (let i = 0; i < cardtabs.length; i++) {
        string += `<h6>${cardtabs[i].innerHTML}</h6>`;
        string += content[i].innerHTML;
    }
    return string;
}
function book(id: number) {
    $.ajax({
        type: 'POST',
        url: "./Reservation",
        data: {
            id: id,
            __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
        },
        success: function (d, _) {
                window.location.href = d;
        }
    });
}
//Open dates
function agendar2(mdId: number) {
    let minDate = $("#MinDate").val();
    let last = $("#Last").val();
    let lastYr = moment(last).toDate().getFullYear();
    if (isNullOrWhitespace(minDate as string)) {
        minDate = moment(dateToday).format(ASPNETDateFormat);
    } else {
        minDate = moment(minDate).format(ASPNETDateFormat);
    }
    let maxDate = $("#MaxDate").val();
    if (isNullOrWhitespace(maxDate as string)) {
        maxDate = moment(last).format(ASPNETDateFormat);
    } else {
        maxDate = moment(maxDate).format(ASPNETDateFormat);
    }
    var dets = $(places[mdId]['card']).removeClass('m6').removeClass('l4');
    dets.find('.card-reveal, .card-action, .tab:first, .time').remove();
    dets.find('.activator').attr('onclick', null).removeClass('activator');
    dets.find('.card').addClass('m-0');
    $('#date-details').html(dets[0].outerHTML);
    $.post("/Patients/Search/GetDates", {
        __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
        mdId: mdId,
        minDate: minDate,
        maxDate: maxDate,
    }, (dt: number[]) => {
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
                $('#dateDetails').html(moment(d).format('dddd DD [de] MMMM, YYYY'));
                var result = moment(d, moment.ISO_8601).format(ASPNETDateFormat);
                $.post("/Patients/Search/TimeSlots", {
                    __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val(),
                    startDate: result,
                    mdId: mdId
                }, function (data: TimeSlotsVM[]) {
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
                        $('#toggle-view').html("ver mapa");
                        //trigger reservation on time selection
                        $("#date-view button.time-select").click(function () {
                            book($(this).data('id'));
                        });
                        M.Tabs.init(document.querySelectorAll('#date-view .tabs'));
                        M.Tooltip.init(document.querySelectorAll('#date-view .tooltipped'));
                });
            }
        });
        datePicker.open();
        return false;
    });
}
//REDIRECT TO DOCTOR DETAILS
function agendar(run: number, mdId: number) {
    //, #MinTime, #MaxTime
    var q = $("#Insurance, #Ubicacion")
        .serialize();
    var date = $("#MinDate,#MaxDate").serializeArray();
    var array: string[] = [q];
    array.push(`MdId=${mdId}`);
    array.push(`Last=${$('#Last').val()}`);
    date.forEach((value) => {
        if (value.value === '') return;
        array.push(value.name + '=' + moment(value.value).format(ASPNETDateFormat));
    });
    window.location.href = `/Patients/Search/DoctorDetails/${run}?` + array.join("&");
    return false;
}
var places: { [cid: string]: { [type: string]: string; } } = {};
function fillReveal(mdId: number) {
    $(`#${mdId}reveal`).html(getContent(mdId));
}
var tmp = [];
document.addEventListener('DOMContentLoaded', function () {
    //change dates
    function changeDates(event: any, inst: any) {
        $('#Dates').find('option').remove().end();
        inst.getVal().forEach((e: Date, _: number) => {
            $('#Dates').append(new Option('', moment(e).format(ASPNETDateFormat), null, true));
        });
        $('#Dates').change();
    }
    //change times
    function changeTimes(event: any, inst: any) {
        tmp.push(event);
        tmp.push(inst);
        var times = inst.getVal();
        if (times != null) {
            $('#MinTime').val(moment(times[0]).format('HH:mm:ss')).change();
            $('#MaxTime').val(moment(times[1]).format('HH:mm:ss')).change();
        }
    }
    //Config button
    M.FloatingActionButton.init(document.getElementById('filter-options'), { toolbarEnabled: true });
    //MOBISCROLL TEST
    mobiscroll.settings = {
        lang: 'es',
        theme: 'material'
    };
    var min = moment(dateToday).startOf('day');
    var max = moment($("#Last").val()).endOf('day');

    function setDay(e: any) {
        var $class = $(e.target).attr('class');
        var num = parseInt($class[$class.length - 1]);
        var isoD = num + 7;
        var y = $('.mbsc-cal-year').attr('aria-label');
        var m = $('.mbsc-cal-month').attr('aria-label');
        var date = moment().set('year', y).set('month', m).set('date', 1).isoWeekday(isoD);
        var month = date.month();
        if (date.date() > 7) date.isoWeekday(num - 7);
        var dates = [];
        while (date.month() == month) {
            if (date >= min && date <= max)
                dates.push(date.toDate());
            date.add(7, 'days');
        }
        calendar.setVal(dates);
    }
    function setWeek(e: any) {
        var num = parseInt($(e.target).html());
        var y = $('.mbsc-cal-year').attr('aria-label');
        var m = $('.mbsc-cal-month').attr('aria-label');
        var date = moment().set('year', y).set('month', m).set('date', 1);
        var month = date.month();
        date.add(7 * (num - 1), 'days');
        var dates = [];
        for (var i = 1; i < 8; i++) {
            date.isoWeekday(i);
            if (date >= min && date <= max && date.month() == month)
                dates.push(date.toDate());
        }
        calendar.setVal(dates);
    }

    function listenDay() {
        for (var i = 0; i < 7; i++) {
            $('.mbsc-cal-week-day' + i).click(setDay);
        }
    }

    function listenWeek() {
        $('.mbsc-cal-week-nr').click(setWeek);
    }

    function setListeners() {
        listenDay(); listenWeek();
    }

    var calendar = mobiscroll.calendar('#demo-max-days', {
        theme: 'material',
        display: 'inline',
        select: 7,
        min: min.toDate(),
        headerText: 'Seleccione hasta 7 dias',
        weekCounter: 'month',
        counter: true,
        showOuterDays: false,
        max: max.toDate(),
        onInit: setListeners,
        onPageLoaded: listenWeek,
        onMonthChange: setListeners,
        onSetDate: changeDates
    });
    mobiscroll.range('#demo', {
        theme: 'material',
        startInput: '#StartTime',
        endInput: '#EndTime',
        minRange: 1000 * 60 * 10,
        steps: {
            minute: 10,
        },
        fromText: 'Desde',
        toText: 'Hasta',
        display: 'inline',
        controls: ['time'],
        hourText: 'Hora',
        minuteText: 'Minutos',
        ampmText: 'Horario',
        amText: 'AM',
        pmText: 'PM',
        timeFormat: 'h:ii A',
        onSetDate: changeTimes,
        defaultValue: [
            "00:00:00",
            "23:59:59"
        ]
    });
    //define list doctor details tabs
    var tabs = ['time', 'addr', 'price', 'mis', 'esp'];
    //initialize sheet modal for ios picker
    M.Modal.init(document.getElementById('select-modal'));
    //initialize ios picker
    $("#example-picker").picker({
        data: ['Próx. Hora Disponible', 'Dirección', 'Valor particular', 'Convenios', 'Especialidad médica']
    }, () => {
        var scrollAmmount = Math.round($('.clone-scroller').scrollTop() / 30);
        var html = $($(".picker-scroller").find(".option").get(scrollAmmount)).html();
        $('#tab-shown').html(html);
        $(`li.tab a`).removeClass("active");
        var $el = $(`li.tab a.${tabs[scrollAmmount]}`).addClass("active");
        var pos = 0;
        if (scrollAmmount > 2) pos += 200;
        $el.parent().parent().animate({ scrollLeft: pos });
        M.Tabs.init(document.querySelectorAll('.tabs'));
    });
});

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
    //destroy and create tabs in sidenav
    var slide_action = M.Sidenav.init(document.getElementById('slide-action'), {
        draggable: true,
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
    //ADD EVENT LISTENER
    function addEventListener(marker: google.maps.Marker, cid: string) {
        google.maps.event.addListener(marker, 'click', _ => {
            $("#slide-header").html(places[cid]["header"]);
            $("#slide-content").html(places[cid]["content"]);
            document.querySelectorAll("#slide-action div.card-tabs, #slide-action ul").forEach((e) => {
                e.addEventListener('touchstart', function () {
                    slide_action.options.draggable = false;
                });
                e.addEventListener('touchend', function () {
                    slide_action.options.draggable = true;
                });
            });
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
            title = `Dr${item.sex ? "" : "a"}. `;
            esp += `<label>Esp.</label><span>${item.especialidad}</span>`;
        }
        var proper = `${title}${item.dr}`;
        esp += '</div>';
        var office = `<p class="address">${place.name}<br/>${place.address}`;
        office += isNullOrWhitespace(item.office) ? '</p>' : `<br/><span>${item.office}</span></p>`;
        return `<div id=${item.cardId} class="col s12 m6 l4">`
            + '<div class="card horizontal sticky-action">'
            + '<div class="card-image">'
            + `<img src="/home/getimg/${item.run}"/>`
            + '</div>'
            + '<div class="card-stacked">'
            + '<div class="card-content">'
            + `<span class="card-title activator" onclick="fillReveal(${item.cardId})">${proper}</span>`
            + '</div>'
            + '<div class="card-tabs">'
            + '<ul class="tabs tabs-fixed-width">'
            + `<li class="tab"><a class="time" href="#${item.cardId}next">Prox. Hora</a></li>`
            + `<li class="tab"><a class="addr" href="#${item.cardId}addr">Dirección</a></li>`
            + `<li class="tab"><a class="price" href="#${item.cardId}prce">Valor Particular</a></li>`
            + conv_tab
            + esp_tab
            + '</ul>'
            + '</div>'
            + '<div class="card-content grey lighten-4">'
            + `<div id="${item.cardId}next" class="tab-content time">${item.nextTS.hora}<br/><a href="#" onclick="book(${item.nextTS.id})">AGENDAR</a></div>`
            + `<div id="${item.cardId}addr" class="tab-content addr">${office}</div>`
            + `<div id="${item.cardId}prce" class="tab-content price"><span><i class="material-icons left">attach_money</i>${format(item.price)}</span></div>`
            + convenios
            + esp
            + '</div>'
            + '<div class="card-action center">'
            + `<a href="#" onclick="agendar2(${item.cardId})"><i class="material-icons">add</i>Otra Fecha/Hr</a>`
            + '</div>'
            + '</div>'
            + `<div id="${item.cardId}reveal" class="card-reveal">`
            + '</div>'
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
                + '<div class="card m-0">'
                + '<div class="card-image">'
                + `<img class="activator placeimg" src="https://maps.googleapis.com/maps/api/place/photo?maxwidth=300&photoreference=${value.place.photoId}&key=AIzaSyDkCLRdkB6VyOXs-Uz_MFJ8Ym9Ji1Xp3rA">`
                + `<span class="card-title activator"><b>${value.place.name}</b></span>`
                + '</div>'
                + '<div class="card-content">'
                + '<span class="card-title activator grey-text text-darken-4">Ver detalles<i class="material-icons right">more_vert</i></span>'
                + '</div>'
                + '<div class="card-reveal">'
                + `<span class="card-title grey-text text-darken-4">${value.place.name}<i class="material-icons right">close</i></span>`
                + `<p><b>Dirección</b>: ${value.place.address}.<a href="https://maps.google.com/?cid=${value.place.cId}"><i class="material-icons">location_on</i></a></p>`
                + '</div>'
                + '</div></div></div>';
            places[value.place.cId]["list"] =
                `<div class="col s12"><h5>${value.place.name}</h5><p>${value.place.address}</p><hr/></div><div id="${value.place.cId}" class="list-place"></div>`
        }
        //LIST
        $("#list").append(places[value.place.cId]["list"]);
        var cnt = 0;
        var match = 0.5;
        var content: string = '';
        $.each(value.items, function () {
            if (!(this.dr in dt)) dt[this.dr] = `/home/getimg/${this.run}`;
            var card = makeCard(this, value.place)
            if (!(this.cardId.toString() in places)) {
                places[this.cardId.toString()] = {};
                places[this.cardId.toString()]['card'] = card;
            }
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
            M.toast({
                html: '<i class="material-icons left">report</i>No hay resultados para los filtros seleccionados<i class="material-icons right">not_interested</i>',
                classes: 'red'
            });
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
                    //show controls
                    $('#map-controls').show();
                    //enable buttons
                    $('button').removeAttr('disabled');
                    //init search bar with autocomplete
                    $("#search-filter").prop('disabled', false);
                    var search = document.getElementById('search-filter');
                    M.Autocomplete.init(search, {
                        data: dt,
                        limit: 10,
                        onAutocomplete: (val) => {
                            var run = parseInt(dt[val].replace(/.*\//g, '').replace(/\..*/g, ''));
                            agendar(run, null);
                        }
                    });
                    //search bar close button
                    $('#close-search').click(_ => {
                        $(search).val('');
                    });
                    //remove old clusters
                    if (typeof (markerCluster) !== 'undefined') {
                        markerCluster.clearMarkers();
                    }
                    //init MARKER CLUSTERER
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
                }
            });
    }
    //SEARCH FUNCTIONS
    function getAllquerys() {
        var array = $("#filter input[name='__RequestVerificationToken'],"
            + "#Insurance, #Ubicacion, #Especialidad, #Sex, #HighlightInsurance, #Dates, #MinTime, #MaxTime")
            .serializeArray();
        return array;
    }
    //GET DATA
    function getData() {
        loaderStart();
        $("#search-filter").prop('disabled', true);
        $('#map-controls').hide();
        $('button').prop('disabled', true);
        $.post("/Patients/Search/MapList", getAllquerys(), getList);
    }
    getData();

    //Move to new location
    var me: google.maps.Marker;
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
    document.getElementById('toggle-center').onclick = function () {
        var icon = document.querySelector('#toggle-center i');
        if (icon.innerHTML == "location_off") {
            icon.innerHTML = "location_on";
            moveToLocation();
        } else {
            icon.innerHTML = "location_off";
            markers.pop();
            me.setMap(null);
            fitToMarkers();
        }
    };
    //RE CENTER MAP
    document.getElementById('re-center').onclick = () => {
        fitToMarkers();
        document.querySelector('#re-center > i').innerHTML = "my_location";
    };
    //DETECT MAP NOT CENTERED
    google.maps.event.addListener(map, 'dragend',
        () => {
            document.querySelector('#re-center > i').innerHTML = "location_disabled";
        });
    //TOGGLE LIST/MAP
    $('#toggle-view').click(function () {
        //hide date view
        $("#date-view").slideUp();
        if ($("#map-view").css("display") === "block") {
            $(this).html("ver mapa");
            $.each(places, function (i) {
                $(`#${i}`).html(this["content"])
            });
            M.Tooltip.init(document.querySelectorAll('#list .tooltipped'));
            $("#map-view").slideUp();
            $("#list-view").slideDown();
            M.Tabs.init(document.querySelectorAll('#list .tabs'));
        } else {
            $("#map-view").slideDown();
            $("#list-view").slideUp();
            $(this).html("ver lista");
            setTimeout(fitToMarkers, 400);
            setTimeout(() => {
                [].forEach.call(document.querySelectorAll('#list .tabs'), (tab: Element) => {
                    M.Tabs.getInstance(tab).destroy();
                });
                [].forEach.call(document.querySelectorAll('#list .tooltipped'), (tooltip: Element) => {
                    M.Tooltip.getInstance(tooltip).destroy();
                });
                $('#list .list-place').html('');
            }, 200);
        }
    });
    var el = new SimpleBar(document.querySelector('.filters-controls'));
    //tag if filter settings have been changed
    let change = false;
    //re load data if changed
    function changeFilter() {
        if (change) {
            deleteMarkers();
            getData();
            change = false;
        }
    }
    //change upper filter button text to reflect selected filters
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
    //init FILTERS MODAL and reload data on close
    var filters = M.Modal.init(document.getElementById('filters-pane'), {
        onCloseStart: changeFilter
    });
    //close on control button click
    $('.filter-action').click(event => {
        event.preventDefault();
        filters.close();
    });
    //reload page on clear click
    $('#filter-clear').click(event => {
        event.preventDefault();
        window.location.reload();
    });
    //if any input within filter form changes tag true
    $('#filter input').change(_ => {
        change = true;
    });
    $('#Dates').change(_ => {
        change = true;
    });
    //load filter options
    $('.fltbtn').click((panel: any) => {
        var id = $(panel.target).attr('id');
        $('.fltcol').hide();
        filters.open();
        let tag;
        switch (id) {
            case "btnEspecialidad":
                tag = '#especialities-col';
                break;
            case "btnUbicacion":
                tag = '#locations-col';
                break;
            case "btnFechaHora":
                tag = '#dates-col';
                break;
            case "all1":
            case "all2":
                tag = '.fltcol';
                break;
        }
        $(tag).fadeIn();
        $(`${tag} input.select2-search__field`).trigger('keyup');
        $('.sel2open').select2('close');
        el.recalculate();
        el.getScrollElement().scrollTop = 0;
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
            placeholder: "Seleccione sexo del profesional médico"
        }).on('select2:selecting', (e) => { $('#Sex').val(null) }).on('change', () => { change = true; });
        //NORMALIZE SELECT2 INPUT
        $("input.select2-search__field")
            .addClass("browser-default");
    });
}