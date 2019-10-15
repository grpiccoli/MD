$(document).ready(function () {
    let $modal = $('.modal');
    $modal.modal({
        onOpenStart: (trigger) => {
            var url = $(trigger).attr("formaction");
            if (url) {
                $modal.find('.modal-content').load(url);
            }
        }
    });
});