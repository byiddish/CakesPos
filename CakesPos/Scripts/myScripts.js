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
                var productName = product.ProductName.toString();
                //$("#productsInnerDiv").append("<button class=" + '"btn productbtn"' + "><img src=" + "/Uploads/" + product.Image + "></button>");
                $("#productsInnerDiv").append("<button class=" + '"btn productbtn"' + "data-id=" + product.Id + " data-content=" + '"' + productName + '"' + " data-price=" + product.Price + " data-inStock=" + product.InStock + "><img src=" + "/Uploads/" + product.Image + "></button>")
            });
        })
    })

    $("#productsInnerDiv").on('click', '.productbtn', function () {
        var p = $(this);
        var id = p.data("id");
        var productName = p.data("content");
        var inStock = p.data("inStock");
        var price = p.data("price");
        var row = "<tr><td><button class=" + '"btn btn-danger delete"' + ">X</button></td><td>" + productName + "</td><td><input class=" + '"input input-sm"' + " type=" + '"number"' + " value=" + '"1"' +"min="+'"1"'+" /></td><td>$" + price + "</td></tr>";
        $("#orderTable").append(row);
    })

    $("#orderTable").on('click', '.delete', function () {
        var i = $(this).closest('tr').index();
        $("tr").eq(i).remove();
    })
})

var vm = new Vue({
    el: '#leftdiv',
    data: {
        q: 3,
        p: 65
    },
    computed: {
        // a computed getter
        t: function () {
            // `this` points to the vm instance
            return this.q * this.p
        }
    }
})