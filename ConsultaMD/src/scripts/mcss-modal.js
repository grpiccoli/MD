document.addEventListener('DOMContentLoaded', function () {
    var elems = document.querySelectorAll('.modal');
    var instances = M.Modal.init(elems, {
        onOpenStart: function (trigger) {
            var url = $(trigger).attr("formaction");
            if (url) {
                $(elems).find('.modal-content').load(url);
            }
        }
    });
});
//# sourceMappingURL=mcss-modal.js.map