
$(window).load(function () {
    $(".loader").fadeOut("slow");
})

$(document).ajaxStart(function () {
    $(".loader").fadeOut("slow");
});
//$(document).ajaxComplete(function () {
//    $(".loader").css("display", "none");
//});


$(function () {
    var deliveryMethod = "";
    var paymentMethod = "";
    var paid = false;


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
        paid = true;
    })

    $('#creditCard').on('click', function () {
        paymentMethod = "Credit Card";
        paid = true;
    })

    $('#check').on('click', function () {
        paymentMethod = "Check";
        paid = true;
    })

    $('#cod').on('click', function () {
        paymentMethod = "COD";
        paid = false;
    })

    $("#orderSubmitBtn").on('click', function () {
        var discount = parseFloat($('.discount').val());
        var customerId = $('#customerIdCheckout').val();
        var ordersId;
        if ($('input[name=paymentMethod]:checked').val() === "DOC") {
            paid = false;
        }
        else {
            paid = true;
        }

        discount = discount || 0;
        $.post("/home/AddOrder", {
            customerId: $('#customerIdCheckout').val(),
            requiredDate: $('#requiredDate').val(),
            deliveryOpt: $('input[name=deliveryOpt]:checked').val(),
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
            paymentMethod: $('input[name=paymentMethod]:checked').val(),
            discount: discount,
            notes: $('#notes').val(),
            greetings: $('#greetings').val(),
            deliveryNote: $('#orderDeliveryNote').val(),
            paid: paid
        },
            function (orderId) {
                ordersId = orderId;
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
                    }, function () {
                    });
                })
                $.post("/home/GetCustomerById", { id: customerId }, function (customer) {
                    if (customer.Caterer && customer.Email != "") {
                        $.post("/home/CreateInvoice", { customerId: customerId, orderId: ordersId }, function () {

                        })
                    }
                })
            })
    });

    $("#EditOrderSubmitBtn").on('click', function () {
        var orderId = $(this).data('orderid');
        var discount = parseFloat($('.discount').val());
        if ($('input[name=paymentMethod]:checked').val() === "DOC") {
            paid = false;
        }
        else {
            paid = true;
        }

        discount = discount || 0;
        $.post("/home/UpdateOrderById", {
            orderId: orderId,
            customerId: $('#customerIdCheckout').val(),
            requiredDate: $('#requiredDate').val(),
            deliveryOpt: $('input[name=deliveryOpt]:checked').val(),
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
            paymentMethod: $('input[name=paymentMethod]:checked').val(),
            discount: discount,
            notes: $('#notes').val(),
            greetings: $('#greetings').val(),
            deliveryNote: $('#orderDeliveryNote').val(),
            paid: paid
        },
            function () {
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
        alert("Order #" + orderId + " updated successfuly!");
    });

    $(".categorybtn").on('click', function () {
        var c = $(this).data("category");
        $.post("/home/GetProductsByCategory", { categoryId: c }, function (products) {
            $(".productbtn").remove();
            products.forEach(function (product) {
                var productName = product.ProductName.toString();
                $("#productsInnerDiv").append("<button class=" + '"btn productbtn"' + "data-id=" + product.Id + " data-categoryId=" + product.CategoryId + " data-content=" + '"' + productName + '"' + " data-price=" + product.Price + " data-inStock=" + product.InStock + "><img src=" + "/Uploads/" + product.Image + "><br/>" + product.ProductName + " $" + product.Price + "</button>")
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

        RefreshOrder();
    })

    $("#orderTable").on('click', '.delete', function () {
        var i = $(this).closest('tr').index();
        var caterer = $('#catererDiscount').val();
        $("tr").eq(i).remove();
        RefreshOrder();
    })

    $("#orderTable").on('input', function () {
        RefreshOrder();
    });

    $('#refresh').on('click', function () {
        RefreshOrder();
    });

    $('#searchInput').on('input', function () {
        $('.customers').remove();
        $('.message').remove();
        var s = $('#searchInput').val().toString();
        $.post("/home/Search", { search: s }, function (customers) {
            if (s === "" || $.isEmptyObject(customers)) {
                //$('.customers').remove();
                $('#searchTable').append("<h1 class=" + '"' + "message" + '"' + ">No matches found...</h1>");

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
                    if (customer.Email == null) {
                        customer.Email = "";
                    }


                    $('#searchTable').append("<tr class=" + '"customers"' + "><td>" + customer.LastName + " " + customer.FirstName + "</td><td>" + customer.Address + " " + customer.City + " " + customer.State + " " + customer.Zip + "</td><td>" + customer.Phone + "</td><td>" + customer.Cell + "</td><td><button class=" + '"' + "btn btn-info select" + '"' + " data-first=" + '"' + customer.FirstName + '"' + "  data-last=" + '"' + customer.LastName + '"' + "  data-add=" + '"' + customer.Address + '"' + "  data-phone=" + '"' + customer.Phone + '"' + " data-id=" + '"' + customer.Id + '"' + " data-cell=" + '"' + customer.Cell + '"' + " data-caterer=" + '"' + customer.Caterer + '"' + " data-email=" + '"' + customer.Email + '"' + " >" + "Select" + "</button></td></tr>");
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
        var email = $(this).data('email');

        $('#customerHeader').text("");
        $('#customerAddress').text("");
        $('#customerPhone').text("");
        $('#customerId').val("");
        $('#customerIdCheckout').val("");
        $('#discountInput').val("");
        $('#catererIndicator').html("");
        $('#customerEmail').text("");

        $('#customerHeader').append(fistName + " " + lastName);
        $('#customerAddress').append(address);
        $('#customerPhone').append(phone);
        $('#customerId').val(customerId);
        $('#searchCustomerModal').modal('toggle');
        $('.customers').remove();
        $('#searchInput').val("");
        $('#customerIdCheckout').val(customerId);
        $('#catererDiscount').val(caterer);
        if (caterer == true) {
            $('#catererIndicator').html("<span style=" + '"color:blue"' + ">*Caterer*</span>");
        }
        $('#customerEmail').append(email);

        RefreshOrder();
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
            populateOrders(ordersHistory);
        })
    })

    $('#historyTable').on('click', '.viewDetailsBtn', function () {
        $("#table").find("tr:gt(0)").remove();
        $('#paymentDiv').html("");
        var ordersId = $(this).data('orderid');
        var customersId = $(this).data('customerid');
        var caterer = $(this).data('caterer');
        var subtotal = 0;
        var total = 0;
        $('#edit').attr('href', "/home/editOrder?customerId=" + customersId + "&orderId=" + ordersId);
        $('#cancel').attr('data-id', ordersId);
        $('#updateStatusBtn').attr('data-orderid', ordersId);
        //$('#edit').attr('data-orderid', ordersId);
        $.post("/home/GetOrderHistory", { customerId: ordersId, orderId: customersId }, function (ordersHistory) {
            if (ordersHistory.order.DeliveryOption === "Delivery") {
                $('#deliveryPanel').show();
                $('#odDeliveryInfo').html("<h4>" + ordersHistory.order.DeliveryFirstName + " " + ordersHistory.order.DeliveryLastName + "</h4><h4>" + ordersHistory.order.DeliveryAddress + "</h4><h4>" + ordersHistory.order.DeliveryCity + " " + ordersHistory.order.DeliveryState + " " + ordersHistory.order.DeliveryZip + "</h4><h4>" + ordersHistory.order.Phone + "</h4>");
            }
            if (ordersHistory.payments != null) {
                ordersHistory.payments.forEach(function (p) {
                    var date = ConvertJsonDate(p.Date);
                    var note = "(" + p.PaymentNote + ")";
                    if (p.PaymentNote == "") {
                        note = "";
                    }
                    $('#paymentDiv').append("<h5>Payment of: $" + p.Payment1 + " on " + date + " " + note + "</h5>");
                })
            }
            var discount = ordersHistory.order.Discount;
            $('#notesBody').html("<h5>" + ordersHistory.order.Notes + "</h5>");
            $('#greetingsBody').html("<h5>" + ordersHistory.order.Greetings + "</h5>")
            $('#odCustomerInfo').html("<h4>" + ordersHistory.customer.FirstName + " " + ordersHistory.customer.LastName + "</h4><h4>" + ordersHistory.customer.Address + "</h4><h4>" + ordersHistory.customer.City + " " + ordersHistory.customer.State + " " + ordersHistory.customer.Zip + "</h4><h4>Phone: " + ordersHistory.customer.Phone + "</h4><h4>Cell: " + ordersHistory.customer.Cell + "</h4><h5>" + ordersHistory.customer.Email + "</h5>");
            ordersHistory.orderedProducts.forEach(function (orderedProducts) {
                var catererDiscount = 0;
                var catererHtml = "";
                //var d = 0;
                //if (caterer) {
                //    if (orderedProducts.categoryId == 1) {
                //        catererDiscountHtml = "(" + 5 * orderedProducts.quantity + ")";
                //    }
                //    else if(orderedProducts.categoryId==){

                //    }
                //}


                if (caterer === true || caterer === "True") {
                    var t = (parseFloat(orderedProducts.quantity) * parseFloat(orderedProducts.unitPrice));
                    if (orderedProducts.categoryId === 1) {
                        catererDiscount = 5;
                        total += (t - catererDiscount * orderedProducts.quantity);
                    }
                    else if (orderedProducts.categoryId === 2) {
                        catererDiscount = t * 0.1;
                        total += (t - catererDiscount);
                    }
                    else if (orderedProducts.categoryId === 5) {
                        catererDiscount = 2.5;
                        total += (t - catererDiscount * orderedProducts.quantity);
                    }
                    if (orderedProducts.caterer === true || caterer === "True" && orderedProducts.categoryId === 2) {
                        catererHtml = (t + "<span style=" + '"color: red"' + "> (-" + catererDiscount + ")</span>");
                    }
                    else {
                        catererHtml = (t + "<span style=" + '"color: red"' + "> (-" + catererDiscount * orderedProducts.quantity + ")</span>");
                    }
                }
                else {
                    var t = (parseFloat(orderedProducts.quantity) * parseFloat(orderedProducts.unitPrice));
                    total += t;
                    catererHtml = t;
                }


                $('#table').append("<tr><td>" + orderedProducts.productName + "</td><td>" + orderedProducts.unitPrice + "</td><td>" + orderedProducts.quantity + "</td><td>" + catererHtml + "</td></tr>")
                //subtotal += orderedProducts.quantity * orderedProducts.unitPrice;
            })
            $('#odDiscount').html("Discount: " + discount);
            $('#odSubtotal').html("Subtotal: $" + total);
            if (discount >= 1) {
                $('#odTotal').html("Total: $" + (total - discount));
            }
            else {
                $('#odTotal').html("Total: $" + (total - (total * discount)));
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
            populateOrders(ordersHistory);
        })
    });

    $('#filterDeliveryDate').on('change', function () {
        var x = $(this).find("option:selected").val();
        $('.deliveryInfoDiv').remove();
        $('#deliveryAlert').remove();
        $.post("/Home/DeliveryFilter", { x: x }, function (deliveries) {
            populateDeliveries(deliveries);
        })
    });

    function populateDeliveries(deliveries) {
        if ($.isEmptyObject(deliveries)) {
            $('#deliveryAlertDiv').append("<h3 id=" + '"' + "deliveryAlert" + '"' + ">No deliveries for this time period...</h3>");
        }
        for (var i = 0, l = deliveries.length; i < l; i++) {
            var requiredDate = ConvertJsonDate(deliveries[i].order.RequiredDate);
            var firstName = deliveries[i].customer.FirstName;
            var lastName = deliveries[i].customer.LastName;
            var deliveryFirstName = deliveries[i].order.DeliveryFirstName;
            var deliveryLastName = deliveries[i].order.DeliveryLastName;
            var deliveryAddress = deliveries[i].order.DeliveryAddress;
            var deliveryCity = deliveries[i].order.DeliveryCity;
            var deliveryState = deliveries[i].order.DeliveryState;
            var deliveryZip = deliveries[i].order.DeliveryZip;
            var deliveryPhone = deliveries[i].order.Phone;
            var deliveryNote = deliveries[i].order.DeliveryNote;



            var productsHtml = "";
            var quantity = "";
            var productName = "";

            if ($.isEmptyObject(deliveries[i].orderedProducts)) {
                productsHtml = "No products for this order..."
            }
            else {
                for (var j = 0, l = deliveries[i].orderedProducts.length; j < l; j++) {
                    quantity = "";
                    if (deliveries[i].orderedProducts != null) {
                        quantity = deliveries[i].orderedProducts[j].quantity;
                    }
                    productName = "";
                    if (deliveries[i].orderedProducts != null) {
                        productName = deliveries[i].orderedProducts[j].productName;
                    }
                    productsHtml += "<h5>" + quantity + " - " + productName + "</h5>";
                }
            }

            $('#deliveriesDiv').append("<div class=" + '"' + "deliveryInfoDiv" + '"' + "><div class=" + '"' + "panel panel-info deliveryInnerDiv" + '"' + "><div class=" + '"' + "panel-heading" + '"' + ">" + requiredDate + " " + firstName + " " + lastName + "<button class=" + '"' + "btn btn-group-xs btn-default pull-right min" + '"' + ">-</button></div><div class=" + '"' + "panel-body body" + '"' + "><div class=" + '"' + "deliveryLeftDiv" + '"' + "><h4>" + deliveryFirstName + " " + deliveryLastName + "<br />" + deliveryAddress + "<br />" + deliveryCity + " " + deliveryState + " " + deliveryZip + "<br />" + deliveryPhone + "</h4></div><div class=" + '"' + "deliveryMiddleDiv" + '"' + ">" + productsHtml + "</div><div class=" + '"' + "deliveryRightDiv" + '"' + "><div class=" + '"' + "form-group" + '"' + "><label for=" + '"' + "deliverynote" + '"' + ">Delivery Note:</label><textarea class=" + '"' + "form-control" + '"' + " rows=" + '"' + "3" + '"' + " id=" + '"' + "deliveryNote" + '"' + ">" + deliveryNote + "</textarea></div></div></div></div></div>");
        }
    }


    function populateOrders(ordersHistory) {
        for (var i = 0, l = ordersHistory.length; i < l; i++) {
            var paidHtml = "<td></td>";
            var deliveryHtml = "";
            var requiredDate = ConvertJsonDate(ordersHistory[i].requiredDate);
            var discount = ordersHistory[i].discount;
            var id = ordersHistory[i].id;
            var deliveryOption = ordersHistory[i].deliveryOpt;
            var firstName = ordersHistory[i].firstName;
            var lastName = ordersHistory[i].lastName;
            var customerId = ordersHistory[i].customerId;
            var payments = ordersHistory[i].payments;
            var caterer = ordersHistory[i].caterer;
            var status = "";
            if (ordersHistory[i].status != null) {
                status = ordersHistory[i].status.Status1;
            }
            var p = 0;
            var total = 0;
            var orderTotal = getTotal(id, customerId);
            if (discount < 1) {
                discount = (orderTotal * discount);
                total = parseFloat((orderTotal - discount), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
            }
            else {
                total = parseFloat((orderTotal - discount), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
            }

            payments.forEach(function (payment) {
                p += payment.Payment1;
            })
            var balance = parseFloat(total - p, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
            //var total = 0;
            if (ordersHistory[i].paid || balance <= 0) {
                paidHtml = "<td><span style=" + '"color:green"' + ">Paid </span><span style=" + '"color:green"' + " class=" + '"glyphicon glyphicon-ok"' + "></span></td>";
            }
            else {
                paidHtml = "<td><span style=" + '"color: red"' + ">" + balance + "</span></td>";
            }
            //var orderTotal = getTotal(id);

            //if (discount < 1) {
            //    discount = (orderTotal * discount);
            //    total = (orderTotal - discount);
            //}
            //else {
            //    total = (orderTotal - discount);
            //}

            if (deliveryOption === "Delivery") {
                deliveryHtml = " <span style=" + '"color:orange"' + " class=" + '"glyphicon glyphicon-road"' + "></span>";
            }

            $('#historyTable').append("<tr class=" + '"history"' + "><td>" + lastName + " " + firstName + "</td><td>" + requiredDate + "</td><td>" + deliveryOption + deliveryHtml + "</td><td>" + total + "</td><td>" + status + "</td>" + paidHtml + " <td><button class=" + '"btn btn-info viewDetailsBtn"' + "data-orderid=" + '"' + id + '"' + "data-customerid=" + '"' + customerId + '"' + "data-caterer=" + '"' + caterer + '"' + ">View Details</button><button class=" + '"btn btn-success paymentBtn"' + "data-orderid=" + id + " data-customerid=" + customerId + " data-balance=" + balance + " data-toggle=" + '"modal"' + " data-target=" + '"#paymentModal"' + ">Payment</button></td></tr>");
        }
    }

    function populateCatererOrders(ordersHistory) {
        for (var i = 0, l = ordersHistory.length; i < l; i++) {
            var customerId = ordersHistory[i].customerId;
            var firstName = ordersHistory[i].firstName;
            var lastName = ordersHistory[i].lastName;
            var balance = '$' + parseFloat(ordersHistory[i].balance, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
            var total = '$' + parseFloat(ordersHistory[i].total, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();

            $('#historysTable').append("<tr class=" + '"history"' + "><td>" + lastName + " " + firstName + "</td><td>" + total + "</td><td><span style=" + '"color: red"' + ">" + balance + "</span></td><td><button class=" + '"btn btn-info sendStatement"' + "data-customerid=" + '"' + customerId + '"' + ">Send Statement</button><button class=" + '"btn btn-success paymentBtn"' + " data-customerid=" + customerId + " data-toggle=" + '"modal"' + " data-target=" + '"#paymentModal"' + ">Payment</button></td></tr>");
        }
    }

    function getTotal(id, customerId) {
        var total;
        $.ajaxSetup({ async: false });
        $.post("/home/GetTotalByOrderId", { id: id, customerId: customerId }, function (orderTotal) {

            total = orderTotal;
        })
        $.ajaxSetup({ async: true });
        return total;
    };

    $('#deliveriesDiv').on('click', '.min', function () {
        //$('.toggle', this).slideToggle();
        $(this).toggleClass(".minimized");
    });

    $('#newCustomerSubmit').on('click', function () {
        var caterer = false;
        if ($('#caterer').is(":checked")) {
            caterer = true;
        }
        var firstName = $('#firstName').val();
        var lastName = $('#lastName').val();
        var address = $('#address').val();
        var city = $('#city').val();
        var state = $('#state').val();
        var zip = $('#zip').val();
        var phone = $('#phone').val();
        var cell = $('#cell').val();
        var email = $('#email').val();

        $.post("/home/addcustomer", { firstName: firstName, lastName: lastName, address: address, city: city, state: state, zip: zip, phone: phone, cell: cell, caterer: caterer, email: email }, function () {
            $('#alertNewCustomerAdded').modal();
        })

        $('#customerHeader').text("");
        $('#customerAddress').text("");
        $('#customerPhone').text("");
        $('#customerId').val("");
        $('#customerIdCheckout').val("");
        $('#discountInput').val("");
        $('#catererIndicator').html("");
        $('#customerEmail').text("");

        $('#customerHeader').append(firstName + " " + lastName);
        $('#customerAddress').append(address);
        $('#customerPhone').append(phone);
        $('#customerId').val(customerId);
        //$('#searchCustomerModal').modal('toggle');
        $('.customers').remove();
        $('#searchInput').val("");
        $('#customerIdCheckout').val(customerId);
        $('#catererDiscount').val(caterer);
        if (caterer == true) {
            $('#catererIndicator').html("<span style=" + '"color:blue"' + ">*Caterer*</span>");
        }
        $('#customerEmail').append(email);
    })

    //$("#homeLink").on('click', function () {
    //    window.location.href = "/home/order";
    //})

    //$("#orderHistoryLink").on('click', function () {
    //    window.location.href = "/home/orderHistory";
    //})

    //$("#inventoryLink").on('click', function () {
    //    window.location.href = "/home/inventory";
    //})

    //$("#deliveryLink").on('click', function () {
    //    window.location.href = "/home/delivery";
    //})



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

    $('#historyTable').on('click', '.paymentBtn', function () {
        var customerId = $(this).data('customerid');
        var orderId = $(this).data('orderid');
        var balance = $(this).data('balance');
        $('#processPaymentBtn').attr('data-customerid', customerId);
        $('#processPaymentBtn').attr('data-orderid', orderId);
        $('#processPaymentBtn').attr('data-balance', balance);
    })

    $('#placeOrderBtn').on('click', function () {
        if ($('#customerIdCheckout').val() === "") {
            $('#alertInvalidCustomer').modal();
        }
        else if ($('#orderTable tr').length<2) {
            $('#alertInvalidOrder').modal();
        }
        else{
            $('#checkoutModal').modal();
        }
    })

    $('#processPaymentBtn').on('click', function () {
        var customerId = $(this).data('customerid');
        var orderId = $(this).data('orderid');
        var amount = $('#amountPay').val();
        var note = $('#paymentNote').val();

        $.post("/home/MakePayment", { customerId: customerId, orderId: orderId, amount: amount, note: note }, function () {
            alert("Thank you for the payment!!!");
        })

        $('#amountPay').val("");
        $('#paymentNote').val("");
        location.reload();
    })

    $('#customerAddressCheckbox').change(function () {
        if (this.checked) {
            var customerId = $('#customerIdCheckout').val();
            $.post("/home/GetCustomerById", { id: customerId }, function (customer) {
                $('#deliveryFirstName').val(customer.FirstName)
                $('#deliveryLastName').val(customer.LastName)
                $('#deliveryAddress').val(customer.Address)
                $('#deliveryCity').val(customer.City)
                $('#deliveryState').val(customer.State)
                $('#deliveryZip').val(customer.Zip)
                $('#deliveryPhone').val(customer.Phone)
            })
        }
        else {
            $('#deliveryFirstName').val("")
            $('#deliveryLastName').val("")
            $('#deliveryAddress').val("")
            $('#deliveryCity').val("")
            $('#deliveryState').val("")
            $('#deliveryZip').val("")
            $('#deliveryPhone').val("")
        }
    });

    $('#cancel').on('click', function () {
        var id = $(this).data('id');
        $.post("/home/DeleteOrder", { id: id }, function () {
            window.location.href = ('/home/orderHistory');
            alert("Order #" + id + " was cancelled!");
        })
    })

    $('#updateStatusBtn').on('click', function () {
        var orderId = $(this).data('orderid');
        var status = $('.status:checked').val();
        if (status == undefined) {
            status = "";
        }
        $.post("/home/UpdateStatus", { orderId: orderId, status: status }, function () {
            location.reload();
            alert("Status updated!!!");
        })
    })

    $('#fullAmountCheckbox').change(function () {
        if (this.checked) {
            var balance = $('#processPaymentBtn').data('balance');
            $('#amountPay').val(parseFloat(balance, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString())
        }
        else {
            $('#amountPay').val("");
        }
    })

    //$('#monthMin').on('click', function () {
    //    $('#monthSelectionDiv').slideToggle();
    //});

    //$('#dateMin').on('click', function () {
    //    $('#monthSelectionDiv').slideToggle();
    //});

    
    $('#filterBySelect').on('change', function () {
        var x = $(this).find("option:selected").val();
        if (x === "byMonth") {
            $('#dateInputDiv').hide();
            $('#monthSelectionDiv').slideToggle();
        }
        else {
            $('#monthSelectionDiv').hide();
            $('#dateInputDiv').slideToggle();
        }
    })

    $('#searchDateBtn').on('click', function () {
        if ($('#fromInput').val() === "" || $('#toInput').val() === "") {
            $('#alertEmptyInput').modal();
        }
        else if ($('#fromInput').val() > $('#toInput').val()) {
            $('#alertInvalidInputs').modal();
        }
        else {

            var min = $('#fromInput').val();
            var max = $('#toInput').val();

            $.post("/home/Statements", { min: min, max: max }, function (orders) {
                $("#historysTable").find("tr:gt(0)").remove();
                populateCatererOrders(orders);
            })
        }
    })

    $('#newCustomerBtn').on('click', function () {
        var phone = $('#searchInput').val();
        $('#phone').val(phone);
    })
        

    //$('#edit').on('click', function () {
    //    var customerId = $(this).data('customerid');
    //    var orderid = $(this).data('orderid');
    //    $.post('/home/EditOrder', { customerId: customerId, orderId: orderid }, function (order) {
    //    })
    //})
    //$('#edit').on('click', function () {
    //    var customerId = $(this).data('customerid');
    //    var orderid = $(this).data('orderid');
    //    //window.location.href = ('/home/order');
    //    append(customerId, orderid);
    //})
    //var customerId = $(this).data('customerid');
    //var orderid = $(this).data('orderid');
    //window.location.href = ('/home/order');
    //append(customerId, orderid);
    //$.post('/home/EditOrder', { customerId: customerId, orderId: orderid }, function (order) {
    //f = order.customer.FirstName;
    //l = order.customer.LastName;
    //$('#customerHeader').append(order.customer.FirstName + " " + order.customer.lastName);
    //$('#customerAddress').append(order.customer.Address);
    //$('#customerPhone').append(order.customer.Phone);
    //$('#customerId').val(customerId);
    ////$('.customers').remove();
    ////$('#searchInput').val("");
    ////$('#customerIdCheckout').val(customerId);
    //$('#catererDiscount').val(order.customer.caterer);
    //if (caterer == true) {
    //    $('#catererIndicator').html("<span style=" + '"color:blue"' + ">*Caterer*</span>");
    //}
    //$('#customerEmail').append(order.customer.Email);
    //})
    //append(f, l)
    //})

    //function append(c, o) {
    //    $.post('/home/EditOrder', { customerId: c, orderId: o }, function (order) {
    //        $('#customerHeader').append(order.customer.FirstName + " " + order.customer.lastName)
    //    })
    //}

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

    function RefreshOrder() {
        var total = 0;
        var itemCount = 0;
        var caterer = $('#catererDiscount').val();
        $('#orderTable').find('tr').not(':first').each(function () {
            var catererDiscount = 0;
            var category = $(this).data('categoryid');
            var quantity = $(this).find('input.q').val();
            itemCount += (parseInt(quantity));
            var price = $(this).data('price');
            if (quantity === undefined) {
                quantity === 0;
            }
            if (caterer === "true") {
                var t = (parseFloat(quantity) * parseFloat(price));
                if (category === 1) {
                    catererDiscount = 5;
                    total += (t - catererDiscount * quantity);
                }
                else if (category === 2) {
                    catererDiscount = t * 0.1;
                    total += (t - catererDiscount);
                }
                else if (category === 5) {
                    catererDiscount = 2.5;
                    total += (t - catererDiscount * quantity);
                }
                if (caterer === "true" && category === 2) {
                    $(this).find('.price').html(t + "<span style=" + '"color: red"' + "> (-" + catererDiscount + ")</span>");
                }
                else {
                    $(this).find('.price').html(t + "<span style=" + '"color: red"' + "> (-" + catererDiscount * quantity + ")</span>");
                }
            }
            else {
                var t = (parseFloat(quantity) * parseFloat(price));
                total += t;
                $(this).find('.price').text(t)
            }

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
    }
})






//(function ($) {
//    "use strict";

//    // Options for Message
//    //----------------------------------------------
//    var options = {
//        'btn-loading': '<i class="fa fa-spinner fa-pulse"></i>',
//        'btn-success': '<i class="fa fa-check"></i>',
//        'btn-error': '<i class="fa fa-remove"></i>',
//        'msg-success': 'All Good! Redirecting...',
//        'msg-error': 'Wrong login credentials!',
//        'useAJAX': true,
//    };

//    // Login Form
//    //----------------------------------------------
//    // Validation
//    $("#login-form").validate({
//        rules: {
//            lg_username: "required",
//            lg_password: "required",
//        },
//        errorClass: "form-invalid"
//    });

//    // Form Submission
//    $("#login-form").submit(function () {
//        remove_loading($(this));

//        if (options['useAJAX'] == true) {
//            // Dummy AJAX request (Replace this with your AJAX code)
//            // If you don't want to use AJAX, remove this
//            dummy_submit_form($(this));

//            // Cancel the normal submission.
//            // If you don't want to use AJAX, remove this
//            return false;
//        }
//    });

//    // Register Form
//    //----------------------------------------------
//    // Validation
//    $("#register-form").validate({
//        rules: {
//            reg_username: "required",
//            reg_password: {
//                required: true,
//                minlength: 5
//            },
//            reg_password_confirm: {
//                required: true,
//                minlength: 5,
//                equalTo: "#register-form [name=reg_password]"
//            },
//            reg_email: {
//                required: true,
//                email: true
//            },
//            reg_agree: "required",
//        },
//        errorClass: "form-invalid",
//        errorPlacement: function (label, element) {
//            if (element.attr("type") === "checkbox" || element.attr("type") === "radio") {
//                element.parent().append(label); // this would append the label after all your checkboxes/labels (so the error-label will be the last element in <div class="controls"> )
//            }
//            else {
//                label.insertAfter(element); // standard behaviour
//            }
//        }
//    });

//    // Form Submission
//    $("#register-form").submit(function () {
//        remove_loading($(this));

//        if (options['useAJAX'] == true) {
//            // Dummy AJAX request (Replace this with your AJAX code)
//            // If you don't want to use AJAX, remove this
//            dummy_submit_form($(this));

//            // Cancel the normal submission.
//            // If you don't want to use AJAX, remove this
//            return false;
//        }
//    });

//    // Forgot Password Form
//    //----------------------------------------------
//    // Validation
//    $("#forgot-password-form").validate({
//        rules: {
//            fp_email: "required",
//        },
//        errorClass: "form-invalid"
//    });

//    // Form Submission
//    $("#forgot-password-form").submit(function () {
//        remove_loading($(this));

//        if (options['useAJAX'] == true) {
//            // Dummy AJAX request (Replace this with your AJAX code)
//            // If you don't want to use AJAX, remove this
//            dummy_submit_form($(this));

//            // Cancel the normal submission.
//            // If you don't want to use AJAX, remove this
//            return false;
//        }
//    });

//    // Loading
//    //----------------------------------------------
//    function remove_loading($form) {
//        $form.find('[type=submit]').removeClass('error success');
//        $form.find('.login-form-main-message').removeClass('show error success').html('');
//    }

//    function form_loading($form) {
//        $form.find('[type=submit]').addClass('clicked').html(options['btn-loading']);
//    }

//    function form_success($form) {
//        $form.find('[type=submit]').addClass('success').html(options['btn-success']);
//        $form.find('.login-form-main-message').addClass('show success').html(options['msg-success']);
//    }

//    function form_failed($form) {
//        $form.find('[type=submit]').addClass('error').html(options['btn-error']);
//        $form.find('.login-form-main-message').addClass('show error').html(options['msg-error']);
//    }

//    // Dummy Submit Form (Remove this)
//    //----------------------------------------------
//    // This is just a dummy form submission. You should use your AJAX function or remove this function if you are not using AJAX.
//    function dummy_submit_form($form) {
//        if ($form.valid()) {
//            form_loading($form);

//            setTimeout(function () {
//                form_success($form);
//            }, 2000);
//        }
//    }

//})(jQuery);