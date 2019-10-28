function loaderStart() {
    $(".preloader-background").css("display", "block");
}
function loaderStop() {
    $(".preloader-background").css("display", "none");
}
//# sourceMappingURL=loader.js.map
var debounce = function (func, delay) {
    var inDebounce;
    return function () {
        var context = this;
        var args = arguments;
        clearTimeout(inDebounce);
        inDebounce = setTimeout(function () { return func.apply(context, args); }, delay);
    };
};
//# sourceMappingURL=debounce.js.map