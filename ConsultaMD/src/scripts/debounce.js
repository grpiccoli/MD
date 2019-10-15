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