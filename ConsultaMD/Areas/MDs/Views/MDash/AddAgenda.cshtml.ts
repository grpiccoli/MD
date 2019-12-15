var now = new Date();

mobiscroll.datetime('#StartTime', {
    timeFormat: 'hh:ii A',
    onInit: (_event, inst) => {
        inst.setVal(now, true);
    }
});

mobiscroll.datetime('#EndTime', {
    timeFormat: 'hh:ii A',
    onInit: (_event, inst) => {
        inst.setVal(now, true);
    }
});

mobiscroll.time('#Duration', {
    timeFormat: 'ii',
    onInit: (_event, inst) => {
        inst.setVal(now, true);
    }
});