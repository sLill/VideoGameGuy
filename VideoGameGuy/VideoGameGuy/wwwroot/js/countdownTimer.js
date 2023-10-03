var timerConnection = new signalR.HubConnectionBuilder()
    .withUrl("/countdownTimerHub")
    .withAutomaticReconnect()
    .build();

timerConnection.on("UpdateTimer", (time) => {
    $("#timer").text(time);
});

timerConnection.onreconnecting(error => {
    console.log("reconnecting");
});

timerConnection.onclose(error => {
    console.log("connection closed");
});

function start_fulfilled() {
    console.log("connection accepted");
}

function start_rejected() {
    console.log("connection refused");
}

timerConnection
    .start()
    .then(start_fulfilled, start_rejected)
    .catch(function (err) {
        console.error(err.toString());
        setTimeout(() => start(), 5000);
    });

