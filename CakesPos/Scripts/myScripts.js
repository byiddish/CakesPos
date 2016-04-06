$(function () {
    $("#delivery").on('click', function () {
        $("#deliveryInfoDiv").show();
        $("#pickup").on('click', function () {
            $("#deliveryInfoDiv").hide();
        })

    })

    $(".categorybtn").on('click', function () {
        var c = $(this).data("category");
        
        //$("#cakesDiv").replaceWith()
        //$.post("GetProductsByCategory", { categoryId: c}, function (result) {

        //    $.each(result, function(i, item) {
        //        alert(Product[i].ProductName);
        //    })
        //})
        //var category = parseInt(c);
        $.post("/home/GetProductsByCategory", { categoryId: c }, function (products) {
            $(".productbtn").remove();
            products.forEach(function (product) {
                $("#productsInnerDiv").append("<button class=" + '"btn productbtn"' + "><img src=" + "/Uploads/" + product.Image + "></button>");
                
            });
            //$("#productDiv").append('<h1>Toby is incredible!!!!!!</h1>');
            //$("#productsDiv").show();
        })
    })
})