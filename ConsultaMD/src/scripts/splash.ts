$(document).ready(function () {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|Nexus/i.test(navigator.userAgent)) {
        $(".splash:last").remove();
        $("#splash").addClass("s12").removeClass("hide-on-small-only");
        let slider = $(".splasher").owlCarousel({
            items: 1,
            loop: false,
            margin: 0,
            autoplay: true,
            autoplayTimeout: 800,
            mouseDrag: false,
            touchDrag: false,
            animateIn: 'flipInY',
            animateOut: 'fadeOut',
            onInitialized: counter,
            onTranslated: counter
        });
        function counter(event: any) {
            var element = event.target;
            var items = event.item.count;
            var item = event.item.index + 1;
            if (item === items) {
                $("#splash").fadeOut();
            }
        }
    } else {
        let slider = $(".splasher").owlCarousel({
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
    }
});
