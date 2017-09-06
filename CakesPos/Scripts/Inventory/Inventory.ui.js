$(function () { // wait for document ready
    // build scene
    var controller = new ScrollMagic.Controller();

    // create a scene
    new ScrollMagic.Scene({ triggerElement: ".categoryNav", triggerHook: 0, duration:7000})
        .setPin(".categoryNav", { pushFollowers: false })
        .addTo(controller);
});
