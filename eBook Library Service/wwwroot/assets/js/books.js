function initCountdown(bookId, endDate) {
    var countdownDate = new Date(endDate).getTime();

    var x = setInterval(function () {
        var now = new Date().getTime();
        var distance = countdownDate - now;

        var days = Math.floor(distance / (1000 * 60 * 60 * 24));
        var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);

        document.getElementById("countdown-" + bookId).innerHTML = days + "d " + hours + "h " + minutes + "m " + seconds + "s ";

        if (distance < 0) {
            clearInterval(x);
            document.getElementById("countdown-" + bookId).innerHTML = "EXPIRED";
        }
    }, 1000);
}
