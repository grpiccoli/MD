function getTimeRemaining(endtime) {
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
function initializeClock(id, endtime) {
    var clock = document.getElementById(id);
    var minutesSpan = clock.querySelector('.minutes');
    var secondsSpan = clock.querySelector('.seconds');
    function updateClock() {
        var t = getTimeRemaining(endtime);
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
//# sourceMappingURL=ConfirmPhone.js.map