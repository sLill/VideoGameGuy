var countdownSeconds = 180;
var sessionId = $('#sessionId').val();
var scoreValue = $('#scoreValue').val();
var highestScoreValue = $('#highestScoreValue').val();
var timerConnection = new signalR.HubConnectionBuilder()
    .withUrl("/countdownTimerHub")
    .withAutomaticReconnect()
    .build();


function start_fulfilled() {
    console.log("connection accepted");
    timerConnection.invoke("StartCountdownForUser", { sessionId: sessionId, seconds: countdownSeconds });
}

function start_rejected() {
    console.log("connection refused");
}

timerConnection.on("UpdateTimer", (time) => {
    $("#timer").text(time);

    $.ajax({
        type: 'POST',
        url: '/Descriptions/UpdateTimer',
        data: { timeRemaining: time }
    });

    // Game Over
    if (time == '00:00') {

        $.ajax({
            type: 'POST',
            url: '/Descriptions/GameOver',
            data: { score: scoreValue }
        });

        highestScoreValue = highestScoreValue > scoreValue ? highestScoreValue : scoreValue;

        $('#userInput').val(gameTitle);
        $('#next_Form').css('display', 'none');
        $('#next_Form').prop('disabled', true);
        $('#skipButton').prop('disabled', true);
        $('#userInput').attr('readonly', true);
        $('#userInput').css('opacity', 0.6);
        $('#scoreDisplay').text('Score: ' + 0);
        $('#highestScoreDisplay').text('Highest Score: ' + highestScoreValue);

        $('#timer').addClass('color_red');
        //setTimeout(function () {
        //    $('#timer').removeClass('color_red');
        //}, 500);
    }
});

timerConnection.onreconnecting(error => {
    console.log("reconnecting");
    timerConnection.invoke("StartCountdownForUser", { sessionId: sessionId, seconds: countdownSeconds });
});

timerConnection.onclose(error => {
    console.log("connection closed");
});

function countdownTimer_SubtractTime(amountSeconds) {
    timerConnection.invoke("SubtractTimeForUser", { sessionId: sessionId, seconds: amountSeconds });
}

timerConnection
    .start()
    .then(start_fulfilled, start_rejected)
    .catch(function (err) {
        console.error(err.toString());
        setTimeout(() => start(), 5000);
    });

