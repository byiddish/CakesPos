$(function () {
    $("#delivery").on('click', function () {
        $("#deliveryInfoDiv").show();
        $("#pickup").on('click', function () {
            $("#deliveryInfoDiv").hide();
        })

    })

    $(".categorybtn").on('click', function () {
        var c = $(this).text();
        $("#cakesDiv").hide();
        $("#"+c).show();
        //$("#productDiv").append('<h1>Toby is incredible!!!!!!</h1>');
        //$("#productsDiv").show();
    })
})