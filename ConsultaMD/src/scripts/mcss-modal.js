$(document).ready(function () {
    var $modal = $('.modal');
    $modal.modal({
        onOpenStart: function (trigger) {
            var url = $(trigger).attr("formaction");
            if (url) {
                $modal.find('.modal-content').load(url);
            }
        }
    });
});
//# sourceMappingURL=mcss-modal.js.map