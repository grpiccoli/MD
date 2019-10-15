$(document).ready(function () {
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
});
