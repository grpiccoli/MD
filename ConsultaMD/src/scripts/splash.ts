$(document).ready(function () {
    let splasher = $(".splasher").owlCarousel({
        items: 1,
        loop: false,
        margin: 0,
        autoplay: true,
        autoplayTimeout: 1500,
        mouseDrag: false,
        touchDrag: false,
        animateIn: 'flipInY',
        animateOut: 'fadeOut'
    });
    if (/Identity\/Account\/Login/.test(window.location.href)
        && /Android|webOS|iPhone|iPad|iPod|BlackBerry|Nexus/i
        .test(navigator.userAgent) 
        && $(window).width() < 600) {
        $(".splash:last").remove();
        $("#splash").removeClass("hide-on-small-only");
        function counter(event: any) {
            var items = event.item.count;
            var item = event.item.index + 1;
            if (item === items) {
                $("#splash").fadeOut();
            }
        }
        var data = splasher.data();
        var options = data['owl.carousel'].options;
        options.onInitialized = counter;
        options.onTranslated = counter;
    }
});
