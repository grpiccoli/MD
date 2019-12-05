mobiscroll.settings = {
    lang: 'es',
    theme: 'material'
};

var now = new Date();

mobiscroll.datetime('#StartTime', {
    timeFormat: 'hh:ii A',
    onInit: function (_event, inst) {
        inst.setVal(now, true);
    }
});

mobiscroll.datetime('#EndTime', {
    timeFormat: 'hh:ii A',
    onInit: function (_event, inst) {
        inst.setVal(now, true);
    }
});

mobiscroll.time('#Duration', {
    timeFormat: 'ii',
    onInit: function (_event, inst) {
        inst.setVal(now, true);
    }
});

var register = mobiscroll.popup('#addAgenda', {
    display: 'center',
    buttons: [{
        text: 'Añadir agenda',
        handler: 'set'
    }]
});

document
    .getElementById('showAdd')
    .addEventListener('click', function () {
        register.show();
    }, false);

var inst = mobiscroll.eventcalendar('#demo-event-popover', {
    lang: 'es',
    theme: 'material',
    themeVariant: 'light',
    display: 'inline',
    showEventCount: true
});

mobiscroll.util.getJson('https://trial.mobiscroll.com/events/', function (events: any) {
    inst.setEvents(events);
}, 'jsonp');