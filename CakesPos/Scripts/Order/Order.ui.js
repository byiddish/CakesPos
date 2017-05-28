$(function () {
    function PlaySound() {
        var sound = document.getElementById("audio");
        sound.play()
    }

    $('body').on('click', '.productbtn', function () {
        PlaySound();
    })
});