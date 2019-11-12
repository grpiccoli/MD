document.addEventListener('DOMContentLoaded', function () {
    const feedback = new signalR.HubConnectionBuilder()
        .withUrl("/feedbackHub")
        .build();

    feedback.on("FeedBack", (msg) => {
        M.toast({ html: msg, classes: 'rounded' });
    });

    feedback.start().then(() => {
        $("#Input_ConnectionId").val(feedback.connection.transport.webSocket.url.split("=")[1]);
    }).catch(err => console.error(err.toString()));
});