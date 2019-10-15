function getTimeRemaining(endtime: string) {
    var t = Date.parse(endtime) - Date.parse(new Date().toString());
    var seconds = Math.floor((t / 1000) % 60);
    var minutes = Math.floor((t / 1000 / 60) % 60);
    var hours = Math.floor((t / (1000 * 60 * 60)) % 24);
    var days = Math.floor(t / (1000 * 60 * 60 * 24));
    return {
        'total': t,
        'days': days,
        'hours': hours,
        'minutes': minutes,
        'seconds': seconds
    };
}

var cleave = new Cleave('#VerificationCode', {
    blocks: [1, 1, 1, 1, 1, 1],
    uppercase: true
});

function initializeClock(id: string, endtime: string) {
    var clock = document.getElementById(id);
    //var daysSpan = clock.querySelector('.days');
    //var hoursSpan = clock.querySelector('.hours');
    var minutesSpan = clock.querySelector('.minutes');
    var secondsSpan = clock.querySelector('.seconds');

    function updateClock() {
        var t = getTimeRemaining(endtime);

        //daysSpan.innerHTML = t.days.toString();
        //hoursSpan.innerHTML = ('0' + t.hours).slice(-2);
        minutesSpan.innerHTML = ('0' + t.minutes).slice(-2);
        secondsSpan.innerHTML = ('0' + t.seconds).slice(-2);

        if (t.total <= 0) {
            clearInterval(timeinterval);
            $("#clock").slideUp();
            $("#resend").removeClass("disabled");
        }
    }

    updateClock();
    var timeinterval = setInterval(updateClock, 1000);
}

var deadline = moment($("#Wait").val().toString(), moment.HTML5_FMT.DATETIME_LOCAL_MS);
if (deadline.isAfter(moment())) {
    initializeClock('clockdiv', deadline.toString());
    $("#clock").slideDown();
    $("#resend").addClass("disabled");
}