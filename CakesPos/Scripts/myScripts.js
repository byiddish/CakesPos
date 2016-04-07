$(function () {
    $("#delivery").on('click', function () {
        $("#deliveryInfoDiv").show();
        $("#pickup").on('click', function () {
            $("#deliveryInfoDiv").hide();
        })

    })

    $(".categorybtn").on('click', function () {
        var c = $(this).data("category");
        $.post("/home/GetProductsByCategory", { categoryId: c }, function (products) {
            $(".productbtn").remove();
            products.forEach(function (product) {
                $("#productsInnerDiv").append("<button class=" + '"btn productbtn"' + "><img src=" + "/Uploads/" + product.Image + "></button>");

            });
        })
    })

    $(".productbtn").on('click', function () {
        var p = $(this);
        var id = p.data("id");
        var productName = p.data("content");
        var inStock = p.data("inStock");
        var price = p.data("price");
        var row = "<tr><td><button class=" + '"btn btn-danger delete"' + ">X</button></td><td>" + productName + "</td><td><input class=" + '"input input-sm"' + " type=" + '"number"' + " value=" + '"1"' + " /></td><td>" + price + "</td></tr>";
        $("#orderTable").append(row);
    })

    $(".delete").on('click', function () {
        var r = $(this);
        r.parent().parent().remove();
    })
})