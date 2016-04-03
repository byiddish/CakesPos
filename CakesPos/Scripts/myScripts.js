$(function () {
    $("#delivery").on('click', function () {
        $("#deliveryInfoDiv").show();
        $("#pickup").on('click', function () {
            $("#deliveryInfoDiv").hide();
        })

    })
})