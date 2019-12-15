mobiscroll.settings = {
    lang: 'es',
    theme: 'material'
};

//var register = mobiscroll.popup('#addAgenda', {
//    display: 'center',
//    buttons: [
//        'set',
//        {
//            text: 'Añadir agenda',
//            handler: async () => {
//                await fetch('/mds/mdash/addagenda', {
//                    method: 'POST',
//                    headers: new Headers({ 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }),
//                    body: `__RequestVerificationToken=${$('input[name="__RequestVerificationToken"]').val()}`
//                        + `&rut=${rut}`
//                        + `&dv=${dv.replace(/k/, 'K')}`
//                })
//                mobiscroll.toast({ message: 'Agenda añadida' });
//            }
//        }
//    ]
//});

//document
//    .getElementById('showAdd')
//    .addEventListener('click', () => {
//        register.show();
//    }, false);

var inst = mobiscroll.eventcalendar('#demo-event-popover', {
    lang: 'es',
    theme: 'material',
    themeVariant: 'light',
    display: 'inline',
    showEventCount: true
});

//mobiscroll.util.getJson('https://trial.mobiscroll.com/events/', function (events: any) {
//    inst.setEvents(events);
//}, 'jsonp');

mobiscroll.util.getJson('/mds/mdash/agendajson', (events: any) => {
    inst.setEvents(events);
}, 'jsonp');