var momentFormat = "DD MMMM YYYY";
var datepickerFormat = "dd mmmm, yyyy";
//REDIRECT TO DOCTOR DETAILS
function agendar(run: number, placeId: string) {
    var q = $("#Insurance, #Ubicacion, #MinTime, #MaxTime")
        .serialize();
    var date = $("#MinDate,#MaxDate").serializeArray();
    var array: string[] = [q];
    array.push(`placeId=${placeId}`);
    array.push(`Last=${$('#Last').val()}`);
    date.forEach(function (value, index) {
        if (value.value === '') return;
        array.push(value.name + '=' + moment(value.value, momentFormat).toJSON().replace("Z", ""));
    });
    var days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    days.forEach(function (value, index) {
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
        "Particular",
        "FONASA",
        "Banmédica",
        "Colmena",
        "Consalud",
        "CruzBlanca",
        "Nueva Masvida",
        "Vida Tres"
    ];
    //ADD MARKER
    let markers: google.maps.Marker[] = [];
    let markerCluster: MarkerClusterer;
    //fit to markers
    function fitToMarkers() {
        //GET BOUNDS
        var bounds = new google.maps.LatLngBounds();
        for (var i = 0; i < markers.length; i++) {
            bounds.extend(markers[i].getPosition());
        }
        map.fitBounds(bounds);
    }
    //MAP
    var map = new google.maps.Map(document.getElementById('map'), {
        mapTypeControl: false,
        scaleControl: true,
        streetViewControl: false,
        fullscreenControl: false,
        zoomControlOptions: {
            position: google.maps.ControlPosition.RIGHT_CENTER
        }
    });
    var places: { [cid: string]: { [type: string]: string; } } = {};
    //ADD EVENT LISTENER
    function addEventListener(marker: google.maps.Marker, cid: string) {
        google.maps.event.addListener(marker, 'click', function () {
            $("#slide-header").html(places[cid]["header"]);
            $("#slide-content").html(places[cid]["content"]);
            $("#slide-action").sidenav('open');
        });
    }
    // Sets the map on all markers in the array.
    function setMapOnAll(map: google.maps.Map) {
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(map);
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
    function format(n:number) {
        return n.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1.');
    }
    function isNullOrWhitespace(input:string) {
        if (typeof input === 'undefined' || input == null) return true;
        return input.replace(/\s/g, '').length < 1;
    }
    function makeCard(item: ResultVM, placeId: string) {
        var convenios = '';
        if (item.insurances.length !== 0) {
            convenios+=`<label>Convenio${item.insurances.length > 1 ? "s" : ""}:</label><ul>`;
            for (var i = 0; i < item.insurances.length; i++) {
                var mi = mis[item.insurances[i]];
                convenios += `<li><img src="/img/mi/${mi}-icon.min.png"/>${mi}</li>`;
            }
            convenios += "</ul><hr/>";
        }
        var dr = item.especialidad !== null;
        var title = dr ? `Dr${item.sex ? "" : "a"}.` : '';
        var esp = dr ? `<li><label>Esp.</label><span>${item.especialidad}</span></li>` : '';
        var office = isNullOrWhitespace(item.office) ? '': `<li><label>Of.</label><span>${item.office}</span></li>`;
        return '<div class="col s12 m6 l4"><div class="card horizontal sticky-action">'
+`<div class="card-image"><img src="/img/doc/${item.run}.min.jpg"/></div>`
+'<div class="card-stacked">'
+'<div class="card-content">'
+   `<span class="card-title activator">${title} ${item.dr}</span>`
+   `<ul>${esp}${office}</ul><hr/>`
+   '<span class="card-title activator">'
+       '<i class="material-icons left">add</i>Más info'
+   '</span>'
+'</div>'
+ '<div class="card-action center">'
+ `<a href="#" onclick="agendar(${item.run},'${placeId}')">Agendar</a>`
+'</div>'
+'</div>'
+'<div class="card-reveal">'
+   `<span class="card-title"><i class="material-icons right">close</i>${title} ${item.dr}</span>`
+   convenios
+   '<label>Valor Particular:</label>'
+   `<span>$${format(item.price)}</span>`
+'</div></div></div>';
    }
    function addMarkers(value: ResultsVM) {
        if (!(value.place.cId in places)) {
            places[value.place.cId] = {};
            //MARKER SIDE PANEL
            places[value.place.cId]["header"] =
'<div class="col s12 m6">'
+'<div class="card m-0">'
+   '<div class="card-image">'
+       `<img class="activator placeimg" src="https://maps.googleapis.com/maps/api/place/photo?maxwidth=300&photoreference=${value.place.photoId}&key=AIzaSyDkCLRdkB6VyOXs-Uz_MFJ8Ym9Ji1Xp3rA">`
+       `<span class="card-title activator"><b>${value.place.name}</b></span>`
+   '</div>'
+   '<div class="card-content">'
+       '<span class="card-title activator grey-text text-darken-4">Ver detalles<i class="material-icons right">more_vert</i></span>'
+   '</div>'
+   '<div class="card-reveal">'
+       `<span class="card-title grey-text text-darken-4">${value.place.name}<i class="material-icons right">close</i></span>`
+       `<p><b>Dirección</b>: ${value.place.address}.<a href="https://maps.google.com/?cid=${value.place.cId}"><i class="material-icons">location_on</i></a></p>`
+   '</div>'
+'</div></div></div>';
        }
        //LIST
        $("#list").append(`<div class="col s12"><h5>${value.place.name}</h5><p>${value.place.address}</p><hr/></div><div id="${value.place.cId}"></div>`);
        var cnt = 0;
        var match = 0.5;
        var content:string = '';
        $.each(value.items, function (_i, val) {
            var card = makeCard(val, value.place.id)
            content += card;
            cnt++;
            if (match === 0.5 && val.match) match = 1;
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
            alert("No hay resultados para los filtros seleccionados");
            if (typeof (markerCluster) !== 'undefined') {
                markerCluster.clearMarkers();
            }
        }
        $("#list").empty();
        $.each(data,
            function (_index, value) {
                addMarkers(value);
                if (!--count) {
                    if (typeof (markerCluster) !== 'undefined') {
                        markerCluster.clearMarkers();
                    }
                    //MARKER CLUSTERER
                    var mcOptions = {
                        gridSize: 40,
                        imagePath: '/img/cluster/m'
                    };
                    markerCluster = new MarkerClusterer(map, markers, mcOptions);
                    markerCluster.setCalculator(function (markers: google.maps.Marker[], numStyles: number) {
                        var index = 1;
                        var count = 0;
                        for (var i = 0; i < markers.length; i++) {
                            count += markers[i].getZIndex();
                        }
                        var index = Math.round(Math.log(count / 4));
                        if (index > 5) index = 5;
                        return {
                            text: count,
                            index: index
                        };
                    });
                    $('.collapsible').collapsible()
                    fitToMarkers();
                    loaderStop();
                }
            });
    }
    //SEARCH FUNCTIONS
    function getAllquerys() {
        var array = $("#filter input[name='__RequestVerificationToken'],"
            + "#Insurance, #Ubicacion, #Especialidad, #Sex, #HighlightInsurance, #MinTime, #MaxTime")
            .serializeArray();
        var date = $("#MinDate,#MaxDate").serializeArray();
        date.forEach(function (value, index) {
            if (value.value === '') return;
            array.push({
                name: value.name,
                value: moment(value.value, momentFormat).toJSON().replace("Z", "")
            });
        });
        var days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
        days.forEach(function (value, index) {
            array.push({
                name: value,
                value: $(`#${value}`).prop('checked')
            })
        });
        return array;
    }
    //GET DATA
    function getData() {
        $.post("/Patients/Search/MapList", getAllquerys(), getList);
    }
    getData();

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
        if ($("#map-view").css("display") === "block") {
            $(this).html("volver al mapa");
            $.each(places, function (index, value) {
                $(`#${index}`).html(value["content"])
            });
        } else {
            $(this).html("ver como lista");
        }
        $("#map-view").slideToggle();
        $("#list-view").slideToggle();
        setTimeout(fitToMarkers, 400);
    });
    function changeFilter() {
        loaderStart();
        deleteMarkers();
        getData();
    }
    function filterBtnTxt(id: string, el: HTMLElement) {
        var text = id;
        var data = $(el).select2('data');
        if (data.length > 0) {
            var number = data.length > 1 ? ` +${data.length - 1}` : '';
            text = `${data[0].text.substring(0, 8)}&hellip;${number}`;
        }
        $(`#btn${id}`).html(text);
    }

    //NAME FILTER
    let data: { [name: string]: string };
    $.getJSON('/Patients/Search/NameList', function (d) {
        data = d;
        $('#search-filter').autocomplete({
            data: d,
            limit: 10,
            onAutocomplete: function (val) {
                console.log(val);
                var run = parseInt(data[val].replace(/.*\//g, '').replace(/\..*/g,''));
                agendar(run, null);
            }
        });
    })
    //FILTER TIMES
    function filterTimes(value: number, type: number) {
        if (value % 6) {
            return 0;
        } else {
            return 1;
        }
    }

    //TIME SLIDER
    var time_slider = document.getElementById('time-slider');
    noUiSlider.create(time_slider, {
        start: [0, 24],
        connect: true,
        step: 1,
        orientation: 'horizontal',
        margin: 1,
        range: {
            'min': 0,
            'max': 24
        },
        format: {
            to: function (value: number) {
                return Math.ceil(value);
            },
            from: function (value: string) {
                return Number(value.replace(/\D/g, ''));
            }
        },
        pips: {
            mode: 'steps',
            density: 4,
            filter: filterTimes,
            format: {
                to: function (value: number) {
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
                from: function (value: string) {
                    return Number(value.replace(/\D/g, ''));
                }
            }
        }
    }).on('set', function (values: any, handle: any) {
        if (handle) {
            $('#MaxTime').val(values[handle] === 24 ? '' : values[handle]);
        } else {
            $('#MinTime').val(values[handle] === 0 ? '' : values[handle]);
        }
    });
    //DATEPICKER
    var dateToday = new Date();
    $('.datepicker').datepicker({
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
    $('#MinDate').change(function (d) {
        var max = M.Datepicker.getInstance(document.getElementById('MaxDate'));
        max.options.minDate = $(this).val() === '' ?
            dateToday :
            moment($(this).val(), momentFormat).toDate();
    });
    var dates = $("#dates-col").detach();
    $('#dates-pane').modal({
        onOpenEnd: function () {
            $(this.el).find('.dates-div').append(dates);
        },
        onCloseStart: changeFilter
    });

    $.getJSON('/Patients/Search/FilterLists', function (d) {
        //SPECIALTY SELECT
        let especialties = $('#Especialidad').select2({
            theme: "material",
            placeholder: "Seleccione especialidades",
            data: d.esp
        });
        especialties.change(function () {
            filterBtnTxt('Especialidad', this);
        });
        var esp = $("#especialities-col").detach();
        //ESPECIALITIES
        $('#specialties-pane').modal({
            onOpenEnd: function () {
                $(this.el).find('.especialities-div').append(esp);
            },
            onCloseStart: changeFilter
        });
        //LOCATION SELECT
        let locationSel2 = $('#Ubicacion').select2({
            theme: "material",
            placeholder: "Seleccione ubicaciones",
            data: d.loc
        });
        locationSel2.change(function () {
            filterBtnTxt('Ubicacion', this);
        });
        var loc = $("#locations-col").detach();
        //LOCATIONS
        $('#locations-pane').modal({
            onOpenEnd: function () {
                $(this.el).find('.locations-div').append(loc);
            },
            onCloseStart: changeFilter
        });
        //FILTERS MODAL
        $('#filters-pane').modal({
            onOpenEnd: function () {
                $(this.el).find('.especialities-div').append(esp);
                $(this.el).find('.locations-div').append(loc);
                $(this.el).find('.dates-div').append(dates);
            },
            onCloseStart: changeFilter
        }).find('.especialities-div').append(esp)
            .find('.locations-div').append(loc)
            .find('.dates-div').append(dates);
        //SEX SELECT
        let sex = $('#Sex').select2({
            theme: "material",
            maximumSelectionLength: 1,
            placeholder: "Seleccione sexo del profesional médico"
        });
        //NORMALIZE SELECT2 INPUT
        $("input.select2-search__field")
            .addClass("browser-default");
    });
}