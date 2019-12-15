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
//# sourceMappingURL=AddAgenda.cshtml.js.map