mobiscroll.settings = {
    lang: 'es',
    theme: 'material'
};
var inst = mobiscroll.eventcalendar('#demo-event-popover', {
    lang: 'es',
    theme: 'material',
    themeVariant: 'light',
    display: 'inline',
    showEventCount: true
});
mobiscroll.util.getJson('/mds/mdash/agendajson', function (events) {
    inst.setEvents(events);
}, 'jsonp');
//# sourceMappingURL=Agenda.js.map