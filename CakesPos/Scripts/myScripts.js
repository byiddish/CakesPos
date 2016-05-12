$(function () {
    var deliveryMethod = "";
    var paymentMethod = "";


    $("#delivery").on('click', function () {
        $("#deliveryInfoDiv").show();
        $("#pickup").on('click', function () {
            $("#deliveryInfoDiv").hide();
        })

    })

    $("#creditCard").on('click', function () {
        $("#creditCardInfoDiv").show();
        $("#cash").on('click', function () {
            $("#creditCardInfoDiv").hide();
        })

    })

    $('#pickup').on('click', function () {
        deliveryMethod = "Pickup";
    })

    $('#delivery').on('click', function () {
        deliveryMethod = "Delivery";
    })

    $('#cash').on('click', function () {
        paymentMethod = "Cash";
    })

    $('#creditCard').on('click', function () {
        paymentMethod = "Credit Card";
    })

    $("#orderSubmitBtn").on('click', function () {
        var discount = parseFloat($('.discount').val());

        discount = discount || 0;
        $.post("/home/AddOrder", {
            customerId: $('#customerIdCheckout').val(),
            requiredDate: $('#requiredDate').val(),
            deliveryOpt: deliveryMethod,
            deliveryFirstName: $('#deliveryFirstName').val(),
            deliveryLastName: $('#deliveryLastName').val(),
            deliveryAddress: $('#deliveryAddress').val(),
            deliveryCity: $('#deliveryCity').val(),
            deliveryState: $('#deliveryState').val(),
            deliveryZip: $('#deliveryZip').val(),
            phone: $('#deliveryPhone').val(),
            creditCard: $('#creditCardNumber').val(),
            expiration: $('#expiration').val(),
            securityCode: $('#securityCode').val(),
            paymentMethod: paymentMethod,
            discount: discount,
            notes: $('#notes').val(),
            greetings: $('#greetings').val(),
            deliveryNote: $('#deliveryNote').val()
        },
            function (orderId) {
                $('#orderTable').find('tr').not(':first').each(function () {
                    var product = $(this);
                    var productId = product.data('id');
                    var price = product.data('price');
                    var quantity = $(this).find('input.q').val();

                    $.post("/home/AddOrderDetails", {
                        orderId: orderId,
                        productId: productId,
                        unitPrice: price,
                        quantity: quantity
                    }, function () { });
                })
            })
    });

    $(".categorybtn").on('click', function () {
        var c = $(this).data("category");
        $.post("/home/GetProductsByCategory", { categoryId: c }, function (products) {
            $(".productbtn").remove();
            products.forEach(function (product) {
                var productName = product.ProductName.toString();
                $("#productsInnerDiv").append("<button class=" + '"btn productbtn"' + "data-id=" + product.Id + " data-categoryId=" + product.CategoryId + " data-content=" + '"' + productName + '"' + " data-price=" + product.Price + " data-inStock=" + product.InStock + "><img src=" + "/Uploads/" + product.Image + "></button>")
            });
        })
    })

    var catererTotal = 0;

    $("#productsInnerDiv").on('click', '.productbtn', function () {
        var exists = false;
        var p = $(this);
        var id = p.data("id");
        var productName = p.data("content");
        var inStock = p.data("inStock");
        var price = p.data("price");
        var categoryId = p.data("categoryid");
        var caterer = $('#catererDiscount').val();
        //catererTotal += price;
        var row = "<tr data-price=" + price + " data-categoryid=" + categoryId + " data-id=" + id + "><td><button class=" + '"btn btn-danger delete"' + ">X</button></td><td>" + productName + "</td><td><input class=" + '"input input-sm q"' + "v-model=" + '"quantity"' + " type=" + '"number"' + " value=" + '"1"' + "min=" + '"1"' + " /></td><td class=" + '"price"' + ">$" + price + "</td></tr>";
        $('#orderTable').find('tr').not(':first').each(function () {
            if ($(this).data('id') == id) {
                var quantity = $(this).find('input.q').val();
                $(this).find('input.q').val(parseInt(quantity) + 1);
                exists = true;
            }
        })
        if (!exists) {
            $("#orderTable").append(row);
        }
        var total = 0;
        var itemCount = 0;
        $('#orderTable').find('tr').not(':first').each(function () {
            var catererDiscount = 0;
            var category = $(this).data('categoryid');
            var quantity = $(this).find('input.q').val();
            itemCount += (parseInt(quantity));
            var price = $(this).data('price')
            if (quantity === undefined) {
                quantity === 0;
            }
            if (caterer) {
                if (category === 1) {
                    catererDiscount = 5;
                }
                else if (category === 2) {
                    catererDiscount = .1;
                }
            }
            var t = (parseFloat(quantity) * parseFloat(price));
            //if()
            total += t - catererDiscount;
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
        var caterer = $('#catererDiscount').val();
        $("tr").eq(i).remove();
        var total = 0;
        var itemCount = 0;
        $('#orderTable').find('tr').not(':first').each(function () {
            var catererDiscount = 0;
            var category = $(this).data('categoryid');
            var quantity = $(this).find('input.q').val();
            itemCount += (parseInt(quantity));
            var price = $(this).data('price')
            if (quantity === undefined) {
                quantity === 0;
            }
            if (caterer) {
                if (category === 1) {
                    catererDiscount = 5;
                }
            }
            var t = (parseFloat(quantity) * parseFloat(price));
            total += t - catererDiscount;
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
        //var catererDiscount = 0;
        var caterer = $('#catererDiscount').val();

        $('#orderTable').find('tr').not(':first').each(function () {
            var catererDiscount = 0;
            var category = $(this).data('categoryid');
            var quantity = $(this).find('input.q').val();
            itemCount += (parseInt(quantity));
            if (quantity === "")
            { quantity = 1; }
            var price = $(this).data('price')
            if (quantity === undefined) {
                quantity = 1;
            }
            if (caterer) {
                if (category === 1) {
                    catererDiscount = 5;
                }
            }

            var t = (parseFloat(quantity) * parseFloat(price));
            total += t - catererDiscount;
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
        var caterer = $('#catererDiscount').val();
        $('#orderTable').find('tr').not(':first').each(function () {
            var catererDiscount = 0;
            var category = $(this).data('categoryid');
            var quantity = $(this).find('input.q').val();
            itemCount += (parseInt(quantity));
            var price = $(this).data('price')
            if (quantity === undefined) {
                quantity === 0;
            }
            if (caterer) {
                if (category === 1) {
                    catererDiscount = 5;
                }
            }
            var t = (parseFloat(quantity) * parseFloat(price));
            total += t - catererDiscount;
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
        $('.customers').remove();
        var s = $('#searchInput').val().toString();
        $.post("/home/Search", { search: s }, function (customers) {
            if (s === "") {
                $('.customers').remove();
            }
            else {
                customers.forEach(function (customer) {
                    if (customer.Phone == null) {
                        customer.Phone = "";
                    }
                    if (customer.Cell == null) {
                        customer.Cell = "";
                    }
                    if (customer.Address == null) {
                        customer.Address = "";
                    }
                    if (customer.City == null) {
                        customer.City = "";
                    }
                    if (customer.State == null) {
                        customer.State = "";
                    }
                    if (customer.Zip == null) {
                        customer.Zip = "";
                    }

                    $('#searchTable').append("<tr class=" + '"customers"' + "><td>" + customer.LastName + " " + customer.FirstName + "</td><td>" + customer.Address + " " + customer.City + " " + customer.State + " " + customer.Zip + "</td><td>" + customer.Phone + "</td><td>" + customer.Cell + "</td><td><button class=" + '"' + "btn btn-info select" + '"' + " data-first=" + '"' + customer.FirstName + '"' + "  data-last=" + '"' + customer.LastName + '"' + "  data-add=" + '"' + customer.Address + '"' + "  data-phone=" + '"' + customer.Phone + '"' + " data-id=" + '"' + customer.Id + '"' + " data-cell=" + '"' + customer.Cell + '"' + "data-caterer=" + '"' + customer.Caterer + '"' + " >" + "Select" + "</button></td></tr>");
                })
            }
        })
    });

    $('#searchCustomerModal').on('click', '.select', function () {
        var fistName = $(this).data('first');
        var lastName = $(this).data('last');
        var address = $(this).data('add');
        var phone = $(this).data('phone');
        var customerId = $(this).data('id');
        var caterer = $(this).data('caterer');
        //if (caterer) {
        //    caterer = .20;
        //}
        //else {
        //    caterer = "";
        //}

        //if (caterer) {
        //    caterer = t;
        //}
        //else {
        //    caterer = "NonCaterer";
        //}

        $('#customerHeader').text("");
        $('#customerAddress').text("");
        $('#customerPhone').text("");
        $('#customerId').val("");
        $('#customerIdCheckout').val("");
        $('#discountInput').val("");
        //$('#catererDiscount').val(false);

        $('#customerHeader').append(fistName + " " + lastName);
        $('#customerAddress').append(address);
        $('#customerPhone').append(phone);
        $('#customerId').val(customerId);
        $('#searchCustomerModal').modal('toggle');
        $('.customers').remove();
        $('#searchInput').val("");
        $('#customerIdCheckout').val(customerId);
        $('#catererDiscount').val(caterer);
        //$('#discountInput').val(caterer);
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

    $('#searchHistoryInput').on('input', function () {
        var s = $('#searchHistoryInput').val().toString();
        $('.history').remove();
        $.post("/home/HistorySearch", { search: s }, function (ordersHistory) {
            ordersHistory.forEach(function (ordersHistory) {
                var paidHtml = "<td></td>";
                var orderDate = ConvertJsonDate(ordersHistory.orderDate);
                var requiredDate = ConvertJsonDate(ordersHistory.requiredDate);
                var discount = ordersHistory.discount;
                var total = 0;

                $.post("/home/GetTotalByOrderId", { id: ordersHistory.id }, function (orderTotal) {
                    if (discount < 1) {
                        discount = (orderTotal * discount);
                        total = (orderTotal - discount);
                    }
                    else {
                        total = (orderTotal - discount);
                    }

                    if (ordersHistory.paid) {
                        paidHtml = "<td><span style=" + '"color: green"' + ">Paid</span></td>";
                    }
                    else {
                        paidHtml = "<td><span style=" + '"color: red"' + ">Not Paid</span></td>";
                    }

                    $('#historyTable').append("<tr class=" + '"history"' + "><td>" + ordersHistory.lastName + " " + ordersHistory.firstName + "</td><td>" + requiredDate + "</td><td>" + ordersHistory.deliveryOpt + "</td><td>" + total + "</td><td></td>" + paidHtml + " <td><button class=" + '"btn btn-info viewDetailsBtn"' + "data-orderid=" + '"' + ordersHistory.id + '"' + "data-customerid=" + '"' + ordersHistory.customerId + '"' + ">View Details</button><button class=" + '"btn btn-success paymentBtn"' + ">Payment</button></td></tr>");
                })
            })
        })
    })

    $('#historyTable').on('click', '.viewDetailsBtn', function () {
        var ordersId = $(this).data('orderid');
        var customersId = $(this).data('customerid');
        var subtotal = 0;
        var total = 0;
        $.post("/home/GetOrderHistory", { customerId: ordersId, orderId: customersId }, function (ordersHistory) {
            if (ordersHistory.order.DeliveryOption === "Delivery") {
                $('#deliveryPanel').show();
                $('#odDeliveryInfo').html("<h3>" + ordersHistory.order.DeliveryFirstName + " " + ordersHistory.order.DeliveryLastName + "</h3><h3>" + ordersHistory.order.DeliveryAddress + "</h3><h3>" + ordersHistory.order.DeliveryCity + " " + ordersHistory.order.DeliveryState + " " + ordersHistory.order.DeliveryZip + "</h3><h3>" + ordersHistory.order.Phone + "</h3>");
            }
            var discount = ordersHistory.order.Discount;
            $('#notesBody').html("<h5>" + ordersHistory.order.Notes + "</h5>");
            $('#odCustomerInfo').html("<h3>" + ordersHistory.customer.FirstName + " " + ordersHistory.customer.LastName + "</h3><h3>" + ordersHistory.customer.Address + "</h3><h3>" + ordersHistory.customer.City + " " + ordersHistory.customer.State + " " + ordersHistory.customer.Zip + "</h3><h3>" + ordersHistory.customer.Phone + "</h3>");
            ordersHistory.orderedProducts.forEach(function (orderedProducts) {
                $('#table').append("<tr><td>" + orderedProducts.productName + "</td><td>" + orderedProducts.unitPrice + "</td><td>" + orderedProducts.quantity + "</td><td>" + orderedProducts.quantity * orderedProducts.unitPrice + "</td></tr>")
                subtotal += orderedProducts.quantity * orderedProducts.unitPrice;
            })
            $('#odDiscount').html("Discount: " + discount);
            $('#odSubtotal').html("Subtotal: $" + subtotal);
            if (discount >= 1) {
                $('#odTotal').html("Total: $" + (subtotal - discount));
            }
            else {
                $('#odTotal').html("Total: $" + (subtotal - (subtotal * discount)));
            }
        })
        $('#orderDetailsModal').modal('show');
    })

    $('#inventoryBtn').on('click', function () {
        var productId = $(this).data('productid')
        var productName = $(this).data('productName')
        $('#productNameInv').html(productName);
        $('#quantityInv').val()
    })

    //$('[data-toggle="popover"]').popover();

    $('.add').on('click', function () {
        var element = $(this)
        var id = $(this).data('id')
        var amount = $(this).closest('tr').find('.invQuantityInput').val();
        $.post("/Home/UpdateInventory", { id: id, amount: amount }, function () {
            location.reload();
            //var prevStock = parseInt(element.closest('tr').find('.inStock').html())
            //var curStock = prevStock += amount;
            //element.closest('tr').find('.inStock').html(curStock);
        })
    })

    $('#filterDate').on('change', function () {
        var x = $(this).find("option:selected").val();
        $('.history').remove();
        $.post("/Home/OrderHistoryFilter", { x: x }, function (ordersHistory) {
            //ordersHistory.sort(a, b, function () {

            //})
            ordersHistory.forEach(function (ordersHistory) {
                var paidHtml = "<td></td>";
                //var orderDate = ConvertJsonDate(ordersHistory.orderDate);
                var requiredDate = ConvertJsonDate(ordersHistory.requiredDate);
                var discount = ordersHistory.discount;
                var total = 0;

                $.post("/home/GetTotalByOrderId", { id: ordersHistory.id }, function (orderTotal) {
                    if (discount < 1) {
                        discount = (orderTotal * discount);
                        total = (orderTotal - discount);
                    }
                    else {
                        total = (orderTotal - discount);
                    }

                    if (ordersHistory.paid) {
                        paidHtml = "<td><span style=" + '"color: green"' + ">Paid</span></td>";
                    }
                    else {
                        paidHtml = "<td><span style=" + '"color: red"' + ">Not Paid</span></td>";
                    }

                    $('#historyTable').append("<tr class=" + '"history"' + "><td>" + ordersHistory.lastName + " " + ordersHistory.firstName + "</td><td>" + requiredDate + "</td><td>" + ordersHistory.deliveryOpt + "</td><td>" + total + "</td><td></td>" + paidHtml + " <td><button class=" + '"btn btn-info viewDetailsBtn"' + "data-orderid=" + '"' + ordersHistory.id + '"' + "data-customerid=" + '"' + ordersHistory.customerId + '"' + ">View Details</button><button class=" + '"btn btn-success paymentBtn"' + ">Payment</button></td></tr>");
                })
            })
        })
    })

    $(".h").click(function () {
        //$('.toggle', this).slideToggle();
        $(this).toggleClass('minimized');
        $(this).children('.deliveryInnerDiv:first').toggle();
    });

    //$('#filterDeliveryDate').on('change', function () {
    //    var x = $(this).find("option:selected").val();
    //    $.post("/Home/DeliveryFilter", { x: x }, function (deliveries) {

    //        deliveries.forEach(function (deliveries) {
    //            var products= deliveries.orderedProducts.each();
    //            $('#historyTable').append("<div class="+'"deliveryInfoDiv"'+"><div class="+'"panel panel-info deliveryInnerDiv"'+"><div class="+'"panel-heading"'+">"+deliveries.RequiredDate.Value.ToShortDateString()+"</div><div class="+'"panel-body"'+"><h3>"+deliveries.order.DeliveryFirstName+" "+deliveries.order.DeliveryLastName+"<br />"+deliveries.order.DeliveryAddress+"<br />"+deliveries.order.DeliveryCity+" "+ deliveries.order.DeliveryState+" "+deliveries.order.DeliveryZip+"<br /></h3>")

    //            deliveries.orderedProducts.each(p, function(){
    //                $('#historyTable').append(
    //            })
    //            @foreach (CakesPos.Data.OrderDetailsProductModel od in o.orderedProducts)
    //            {
    //                <h4>@od.quantity x @od.productName</h4>
    //        }
    //            <textarea>Deliver on time.....</textarea>
    //        </div>
    //    </div>
    //</div>
    //        })
    //    })

    //})

    $('#print').click(function () {
        window.print();
    });

    function ConvertJsonDate(jsonDate) {
        var jsonDate = jsonDate.toString();
        var value = new Date
                    (
                         parseInt(jsonDate.replace(/(^.*\()|([+-].*$)/g, ''))
                    );
        var date = value.getMonth() +
                                 1 +
                               "/" +
                   value.getDate() +
                               "/" +
               value.getFullYear();
        return date;
    }


})