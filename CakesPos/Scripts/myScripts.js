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
        var row = "<tr data-price=" + price + "><td><button class=" + '"btn btn-danger delete"' + ">X</button></td><td>" + productName + "</td><td><input class=" + '"input input-sm q"' + "v-model=" + '"quantity"' + " type=" + '"number"' + " value=" + '"1"' + "min=" + '"1"' + " /></td><td class=" + '"price"' + ">$" + price + "</td></tr>";
        $("#orderTable").append(row);
        var total = 0;
        var itemCount = 0;
        $('#orderTable').find('tr').not(':first').each(function () {
            var quantity = $(this).find('input.q').val();
            itemCount += (parseInt(quantity));
            var price = $(this).data('price')
            if (quantity === undefined) {
                quantity === 0;
            }
            var t = (parseFloat(quantity) * parseFloat(price));
            total += t;
            $(this).find('.price').text(t);
            $('#totalItems').text("Total items: " + itemCount);
        });
        if (total === NaN) {
            $('#total').text("Total: $" + 0);
        }
        else {
            if (getDiscount() < 1) {
                var d = total * getDiscount();
                $('#total').text("Total: $" + (total - d));
            }
            else {
                $('#total').text("Total: $" + (total - getDiscount()));
            }
        }
    })

    $("#orderTable").on('click', '.delete', function () {
        var i = $(this).closest('tr').index();
        $("tr").eq(i).remove();
        var total = 0;
        var itemCount = 0;
        $('#orderTable').find('tr').not(':first').each(function () {
            //itemCount++;
            var quantity = $(this).find('input.q').val();
            itemCount += (parseInt(quantity));
            var price = $(this).data('price')
            if (quantity === undefined) {
                quantity === 0;
            }
            var t = (parseFloat(quantity) * parseFloat(price));
            total += t;
            $(this).find('.price').text(t);
            $('#totalItems').text("Total items: " + itemCount);
        });
        if (total === NaN) {
            $('#total').text("Total: $" + 0);
        }
        else {
            if (getDiscount() < 1) {
                var d = total * getDiscount();
                $('#total').text("Total: $" + (total - d));
            }
            else {
                $('#total').text("Total: $" + (total - getDiscount()));
            }
        }
    })

    $("#orderTable").on('input', function () {
        var total = 0;
        var itemCount = 0;
        $('#orderTable').find('tr').not(':first').each(function () {
            //itemCount++;
            var quantity = $(this).find('input.q').val();
            itemCount += (parseInt(quantity));
            if (quantity === "")
            { quantity = 1; }
            var price = $(this).data('price')
            if (quantity === undefined) {
                quantity = 1;
            }

            var t = (parseFloat(quantity) * parseFloat(price));
            total += t;
            $(this).find('.price').text(t);
            $('#totalItems').text("Total items: " + itemCount);
        });
        if (total === NaN) {
            $('#total').text("Total: $" + 0);
        }
        else {
            if (getDiscount() < 1) {
                var d = total * getDiscount();
                $('#total').text("Total: $" + (total - d));
            }
            else {
                $('#total').text("Total: $" + (total - getDiscount()));
            }
        }
    });

    $('#refresh').on('click', function () {
        var total = 0;
        var itemCount = 0;
        $('#orderTable').find('tr').not(':first').each(function () {
            //itemCount++;
            var quantity = $(this).find('input.q').val();
            itemCount += (parseInt(quantity));
            var price = $(this).data('price')
            if (quantity === undefined) {
                quantity === 0;
            }
            var t = (parseFloat(quantity) * parseFloat(price));
            total += t;
            $(this).find('.price').text(t);
        });
        $('#totalItems').text("Total items: " + itemCount);
        if (total === NaN) {
            $('#total').text("Total: $" + 0);
        }
        else {
            if (getDiscount() < 1) {
                var d = total * getDiscount();
                $('#total').text("Total: $" + (total - d));
            }
            else {
                $('#total').text("Total: $" + (total - getDiscount()));
            }
        }
    });

    $('#searchInput').on('input', function () {
        //$('#searchTable').remove('.customers');
        $('.customers').remove();
        var s = $('#searchInput').val().toString();
        $.post("/home/Search", { search: s }, function (customers) {
            if (customers == null) {
                $('#message').append("<h1>" + "Sorry no matches:(" + "</h1>");
            }
            else {
                customers.forEach(function (customer) {
                    $('#searchTable').append("<tr class=" + '"customers"' + "><td>" + customer.FirstName + "</td><td>" + customer.LastName + "</td><td>" + customer.Address + "</td><td>" + customer.Phone + "</td><td>" + customer.Caterer + "</td><td><button class=" + '"' + "btn btn-info" + '"' + ">" + "Select" + "</button></td></tr>");
                })
            }
        })
    });

    function getDiscount() {
        var discount = $("#totalDivInner").find('.discount').val();
        if (discount === "") {
            return 0;
        }
        else {
            return (parseFloat(discount));
        }
    }

})