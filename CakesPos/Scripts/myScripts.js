$(function () {
    var deliveryMethod = "";
    var paymentMethod = "";
    var paid = false;
    var elem = "";
    var cId = "";
    var oId = "";
    var t = "";
    var cust = "";
    var orderListFilter = "All";

    var IsLoaderForInput = false;
    var IsLoaderForSubmit = false;

    $(document).ajaxStart(function () {
        if (IsLoaderForInput) {
            $(".cssload-preloader").show();
        }
    });

    $(document).ajaxComplete(function () {
        if (IsLoaderForInput) {
            $(".cssload-preloader").hide();
            // reset the values
            IsLoaderForInput = false;
            IsLoaderForSubmit = false;
        }
    });

    $(function () {
        $('a').on('click', function (e) {
            localStorage.setItem('lastTab', $(this).attr('href'));
        });
    });

    function setTwoNumberDecimal(event) {
        this.value = parseFloat(this.value).toFixed(2);
    }

    var lastTab = localStorage.getItem('lastTab');
    if (lastTab) {
        $('a[href="' + lastTab + '"]').closest('li').addClass('active').siblings().removeClass('active');
    }

    $("#delivery").on('click', function () {
        $("#deliveryInfoDiv").show();
    })

    $("#pickup").on('click', function () {
        $("#deliveryInfoDiv").hide();
    })

    $('#pickup').on('click', function () {
        deliveryMethod = "Pickup";
    })

    $('#delivery').on('click', function () {
        deliveryMethod = "Delivery";
        if ($('#catererDiscount').val() != "true") {
            $('#cod').attr('checked', true);
        }
    })

    $('#cash').on('click', function () {
        paymentMethod = "Cash";
        paid = true;
    })

    $('#creditCard').on('click', function () {
        paymentMethod = "Credit Card";
        paid = false;
    })

    $('#paid').on('click', function () {
        paymentMethod = "Paid";
        paid = true;
    })

    $('#cod').on('click', function () {
        paymentMethod = "COD";
        paid = false;
    })

    $('#atPickup').on('click', function () {
        paymentMethod = "At Pickup";
        paid = false;
    })

    $('#onAccount').on('click', function () {
        paymentMethod = "On Account";
        paid = false;
    })

    $('.customizeBtn').on('click', function () {
        $('#customizeProductModal').modal();
    })

    $('#customProductSubmit').on('click', function () {
        var productName = $('#productName').val();
        var price = $('#price').val();
        var catererDiscount = $('#catererDiscount').val();
        var inStock = $('#inStock').val();
        var restockAmount = $('#restockAmount').val();
        if (productName == "" || price == "" || inStock == "" || catererDiscount == "") {
            $('#invalidSubmitAlert').html("<span style=" + '"color:red"' + ">All inputs are necessary!</span>")
            $('#alertInvalidSubmitModal').modal();
        }
        else {
            $.post("/home/AddCustomProduct", { productName: productName, price: price, catererDiscount: catererDiscount, restockAmount: restockAmount, inStock: inStock, }, function () {
                $('#customizeProductModal').modal('toggle');
                $('#productName').val("");
                $('#price').val("");
                $('#inStock').val("");
            });
        }
    });


    $('#historyTable').on('click', '.viewInvoiceBtn', function () {
        var invoiceId = $(this).data('orderid');
        var customerId = $(this).data('customerid');
        $('#emailInvoiceBtn').data('customerid', customerId);
        $('#emailInvoiceBtn').data('orderid', invoiceId);
        $('#viewPdfIFrame').attr('src', "/home/GetInvoice?invoiceId=" + invoiceId);
        $('#pdfModal').modal();
    })

    $("#orderSubmitBtn").on('click', function () {
        if ($('.requiredDate').datepicker("getDate") === null) {
            $('#invalidSubmitAlert').html("<span style=" + '"color:red"' + ">Please select a required date to continue!</span>")
            $('#alertInvalidSubmitModal').modal();
        }
        else if ($('#delivery').prop('checked') == true && $('#deliveryAddress').val() === "") {
            $('#invalidSubmitAlert').html("<span style=" + '"color:red"' + ">Please enter a delivery address to continue!</span>")
            $('#alertInvalidSubmitModal').modal();
        }
        else if ($('#delivery').prop('checked') == true && $('#deliveryCharge').val() === "") {
            $('#invalidSubmitAlert').html("<span style=" + '"color:red"' + ">Please enter a delivery charge to continue!</span>")
            $('#alertInvalidSubmitModal').modal();
        }
        else {
            var discount = parseFloat($('.discount').val());
            var customerId = $('#customerIdCheckout').val();
            var ordersId;
            if ($('input[name=paymentMethod]:checked').val() === "On Account") {
                paid = true;
            }
            else {
                paid = false;
            }
            discount = discount || 0;
            var deliveryCharge = 0;
            if ($('input[name=deliveryOpt]:checked').val() === "Delivery") {
                deliveryCharge = $('#deliveryCharge').val();
            }
            IsLoaderForInput = true;
            $.post("/home/AddOrder", {
                customerId: $('#customerIdCheckout').val(),
                requiredDate: $('.requiredDate').val(),
                deliveryOpt: $('input[name=deliveryOpt]:checked').val(),
                deliveryFirstName: $('#deliveryFirstName').val(),
                deliveryLastName: $('#deliveryLastName').val(),
                deliveryAddress: $('#deliveryAddress').val(),
                deliveryCity: $('#deliveryCity').val(),
                deliveryState: $('#deliveryState').val(),
                deliveryZip: $('#deliveryZip').val(),
                phone1: $('#deliveryPhone1').val(),
                phone2: $('#deliveryPhone2').val(),
                cell1: $('#deliveryCell1').val(),
                cell2: $('#deliveryCell2').val(),
                creditCard: $('#creditCardNumber').val(),
                expiration: $('#expiration').val(),
                securityCode: $('#securityCode').val(),
                paymentMethod: $('input[name=paymentMethod]:checked').val(),
                discount: discount,
                notes: $('#notes').val(),
                greetings: $('#greetings').val(),
                deliveryNote: $('#orderDeliveryNote').val(),
                paid: paid,
                deliveryCharge: deliveryCharge
            },
                function (orderId) {
                    //if (paid) {
                    //    amount = parseFloat($('#total').data('total'));
                    //    amount += parseFloat(deliveryCharge);
                    //    $.post("/home/MakePayment", { customerId: customerId, orderId: orderId, amount: amount, note: paymentMethod }, function () {
                    //    })
                    //}
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
                            $('#alertOrderAdded').modal();
                            //location.href = location.href;
                        });
                    })
                    //$.post("/home/GetCustomerById", { id: customerId }, function (customer) {
                    //    if (customer.Caterer && customer.Email != "") {
                    //        $.post("/home/CreateInvoice", { customerId: customerId, orderId: ordersId }, function () {

                    //        })
                    //    }
                    //})
                    if ($('input[name=paymentMethod]:checked').val() === "On Account") {
                        var total = parseFloat($('#total').data('total'));
                        total += parseFloat(deliveryCharge);
                        $.post("/home/DeductFromAccount", { customerId: customerId, orderId: orderId, amount: total }, function () {
                        })
                    }
                })
        }
    });

    $('#checkoutModal').on('show.bs.modal', function () {
        if ($('#catererDiscount').val() == "true") {
            $('#billMonthly').attr('checked', true);
        }
    })


    $('#alertOrderAdded').on('hidden.bs.modal', function () {
        location.href = location.href;
    })

    $('.o').on('click', function () {
        location.href = location.href;
    });

    $("#EditOrderSubmitBtn").on('click', function () {
        if ($('.requiredDate').datepicker("getDate") === null) {
            $('#invalidSubmitAlert').html("<span style=" + '"color:red"' + ">Please select a required date to continue!</span>")
            $('#alertInvalidSubmitModal').modal();
        }
        else if ($('#delivery').prop('checked') == true && $('#deliveryAddress').val() === "") {
            $('#invalidSubmitAlert').html("<span style=" + '"color:red"' + ">Please enter a delivery address to continue!</span>")
            $('#alertInvalidSubmitModal').modal();
        }
        else if ($('#delivery').prop('checked') == true && $('#deliveryCharge').val() === "") {
            $('#invalidSubmitAlert').html("<span style=" + '"color:red"' + ">Please enter a delivery charge to continue!</span>")
            $('#alertInvalidSubmitModal').modal();
        }
        else {
            var orderId = $(this).data('orderid');
            var discount = parseFloat($('.discount').val());
            var deliveryCharge = $('#deliveryCharge').val();
            if ($('input[name=deliveryOpt]:checked').val() === "Pickup") {
                deliveryCharge = 0;
            }
            if ($('input[name=paymentMethod]:checked').val() === "On Account") {
                paid = true;
            }
            else {
                paid = false;
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
                phone1: $('#deliveryPhone1').val(),
                phone2: $('#deliveryPhone2').val(),
                cell1: $('#deliveryCell1').val(),
                cell2: $('#deliveryCell2').val(),
                creditCard: $('#creditCardNumber').val(),
                expiration: $('#expiration').val(),
                securityCode: $('#securityCode').val(),
                paymentMethod: $('input[name=paymentMethod]:checked').val(),
                discount: discount,
                notes: $('#notes').val(),
                greetings: $('#greetings').val(),
                deliveryNote: $('#orderDeliveryNote').val(),
                paid: paid,
                deliveryCharge: deliveryCharge
            },
                function () {
                    var amount = parseFloat($('#total').data('total'));
                    amount += parseFloat(deliveryCharge);
                    if ($('#paymentMethodIndicator').val() == "On Account" && $('input[name=paymentMethod]:checked').val() != "On Account") {
                        $.post("/home/MakeAccountTrans", { customerId: $('#customerIdCheckout').val(), amount: amount, note: "Order #" + orderId + " Refund" }, function () {
                        })
                    }
                    else if ($('#paymentMethodIndicator').val() != $('input[name=paymentMethod]:checked').val() && $('input[name=paymentMethod]:checked').val() == "On Account") {
                        //$.post("/home/DeletePayments", { orderId: orderId }, function () {
                        //})
                        $.post("/home/MakeAccountTrans", { customerId: $('#customerIdCheckout').val(), amount: -amount, note: "Order #" + orderId + " Withdrawal" }, function () {
                        })
                    }
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
            $('#alertOrderChanges').modal();
        }
    });

    $('.sendStatementBtn').on('click', function () {
        var customerId = $(this).data('customerid');
        $('#emailStatementBtn').data('customerid', customerId);
    })

    $('.emailStatementBtn').on('click', function () {
        var $btn = $(this).button('loading')
        var customerId = $(this).data('customerid');
        var tr = $(this);
        $.post("/home/GetCustomerById", { id: customerId }, function (customer) {
            if (customer.Caterer && customer.Email != "" && customer.Email != null) {
                $.post("/home/GenerateStatementEmail", { customerId: customerId }, function () {
                    $btn.button('reset')
                    tr.closest('tr').remove();
                    $('#statementAlertMessage').html('A statement was sent via email to the customer!');
                    $('#statementAlertModal').modal();
                })
            }
            else {
                $btn.button('reset')
                $('#statementAlertMessage').html("<span style=" + '"' + "color:red" + '"' + ">This customer does not have an email address on file...</span>")
                $('#statementAlertModal').modal();
            }
        })
    })

    $('.printAndEmail').on('click', function () {
        var $btn = $(this).button('loading')
        var customerId = $(this).data('customerid');
        var tr = $(this);
        $.post("/home/GetCustomerById", { id: customerId }, function (customer) {
            if (customer.Caterer && customer.Email != "" && customer.Email != null) {
                $.post("/home/GenerateStatementPrintEmail", { customerId: customerId }, function (id) {
                    $btn.button('reset')
                    tr.closest('tr').remove();
                    $('#statementAlertMessage').html('A statement was sent via email to the customer!');
                    $('#statementAlertModal').modal();
                    printStatement(id);
                })
            }
            else {
                $btn.button('reset')
                $('#statementAlertMessage').html("<span style=" + '"' + "color:red" + '"' + ">This customer does not have an email address on file...</span>")
                $('#statementAlertModal').modal();
            }
        })
    })

    $('.printStatement').on('click', function () {
        var $btn = $(this).button('loading')
        var customerId = $(this).data('customerid');
        var tr = $(this);
        $.post("/home/GenerateStatementPrint", { customerId: customerId }, function (id) {
            printStatement(id);
            $btn.button('reset')
            tr.closest('tr').remove();
        })
    });

    $('#billsTab').on('click', function () {
        $('#billsTab').attr('class', 'active');
        $('#statementsTab').attr('class', '');
        $('#containerDiv').remove();
        $('#statementHeader').html("Bills");
        $.get("/home/GetOpenStatements", function (bills) {
            populateBills(bills);
        })
    })

    $('#statementsTab').on('click', function () {
        $('#billsTab').attr('class', '');
        $('#statementsTab').attr('class', 'active');
        $('#containerDiv').remove();
    })

    function getTotalStatementPayments(Payments) {
        var totalPayments = 0;
        $.each(Payments, function (i, v) {
            totalPayments += v.Payment;
        })
        return totalPayments;
    }


    function populateBills(bills) {
        $('#orderDiv').append("<div id=" + '"' + "containerDiv" + '"' + "><label>Search <input class=" + '"' + "input input-lg form-control" + '"' + "type=" + '"' + "text" + '"' + " id=" + '"' + "customerSearch" + '"' + " placeholder=" + '"' + "Customer Name" + '"' + "/></label><select class=" + '"' + "input input-lg" + '"' + " id=" + '"' + "filterBills" + '"' + "><option value=" + '"' + "open" + '"' + ">Open</option><option value=" + '"' + "closed" + '"' + ">Closed</option><option value=" + '"' + "all" + '"' + ">All</option></select></div>");
        $('#containerDiv').append("<div><table class=" + '"' + "table table-hover table-responsive table-striped billsTable" + '"' + "><tr><th>Date</th><th>Customer</th><th>Statement #</th><th>Amount</th><th>Balance</th><th>Action</th></tr></table></div>")
        $.each(bills, function (i, v) {
            var balance = v.Statement.Balance - getTotalStatementPayments(v.Payments);
            $('.billsTable').append("<tr><td>" + ConvertJsonDate(v.Statement.Date) + "</td><td>" + v.Orders[0].customer.LastName.toString() + " " + v.Orders[0].customer.FirstName.toString() + "</td><td>" + v.Statement.Id + "</td><td>" + parseFloat((v.Statement.Balance), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + "</td><td>" + parseFloat((balance), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + "</td><td><button class=" + '"' + "btn btn-info viewPdfBtn" + '"' + " style=" + '"' + "margin-right:5px" + '"' + " data-statementid=" + '"' + v.Statement.Id + '"' + ">View PDF</button><button class=" + '"' + "btn btn-success payStatementBtn" + '"' + " data-statementid=" + '"' + v.Statement.Id + '"' + " data-customerid=" + '"' + v.Orders[0].customer.Id + '"' + " data-balance=" + '"' + balance + '"' + ">Payment</button></td></tr>");

        })
    }

    $('#orderDiv').on('change', '#filterBills', '#customerSearch', function () {
        var filter = $('#orderDiv #filterBills option:selected').val().toString();
        var search = $('#orderDiv #customerSearch').val().toString();
        $('.billsTable').find("tr:gt(0)").remove();
        $('#billsAlert').remove();
        $('#statementHeader').html("Bills");
        $.post("/home/GetStatementsFiltered", { search: search, filter: filter }, function (bills) {
            if (bills.length === 0) {
                $('#containerDiv').append("<h3 id=billsAlert>No matches found!!...</h3>")
            }
            //$('#containerDiv').append("<div><table class=" + '"' + "table table-hover table-responsive table-striped billsTable" + '"' + "><tr><th>Date</th><th>Customer</th><th>Statement #</th><th>Amount</th><th>Balance</th><th>Action</th></tr></table></div>")
            $.each(bills, function (i, v) {
                var balance = v.Statement.Balance - getTotalStatementPayments(v.Payments);
                if (balance <= 0) {
                    $('.billsTable').append("<tr><td>" + ConvertJsonDate(v.Statement.Date) + "</td><td>" + v.Orders[0].customer.LastName.toString() + " " + v.Orders[0].customer.FirstName.toString() + "</td><td>" + v.Statement.Id + "</td><td>" + parseFloat((v.Statement.Balance), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + "</td><td class=" + '"' + "balance" + '"' + ">" + parseFloat((balance), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + "</td><td><button class=" + '"' + "btn btn-info viewPdfBtn" + '"' + " style=" + '"' + "margin-right:5px" + '"' + " data-statementid=" + '"' + v.Statement.Id + '"' + ">View PDF</button></td></tr>");
                }
                else {
                    $('.billsTable').append("<tr><td>" + ConvertJsonDate(v.Statement.Date) + "</td><td>" + v.Orders[0].customer.LastName.toString() + " " + v.Orders[0].customer.FirstName.toString() + "</td><td>" + v.Statement.Id + "</td><td>" + parseFloat((v.Statement.Balance), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + "</td><td class=" + '"' + "balance" + '"' + ">" + parseFloat((balance), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + "</td><td><button class=" + '"' + "btn btn-info viewPdfBtn" + '"' + " style=" + '"' + "margin-right:5px" + '"' + " data-statementid=" + '"' + v.Statement.Id + '"' + ">View PDF</button><button class=" + '"' + "btn btn-success payStatementBtn" + '"' + " data-statementid=" + '"' + v.Statement.Id + '"' + " data-customerid=" + '"' + v.Orders[0].customer.Id + '"' + " data-balance=" + balance + ">Payment</button></td></tr>");
                }
            })
        })
    })

    $('#orderDiv').on('input', '#customerSearch', function () {
        var filter = $('#orderDiv #filterBills option:selected').val().toString();
        var search = $('#orderDiv #customerSearch').val().toString();
        $('#billsAlert').remove();
        $('#statementHeader').html("Bills");
        $.post("/home/GetStatementsFiltered", { search: search, filter: filter }, function (bills) {
            $('.billsTable').find("tr:gt(0)").remove();
            if (bills.length === 0) {
                $('#containerDiv').append("<h3 id=billsAlert>No matches found...</h3>")
            }
            //$('#containerDiv').append("<div><table class=" + '"' + "table table-hover table-responsive table-striped billsTable" + '"' + "><tr><th>Date</th><th>Customer</th><th>Statement #</th><th>Amount</th><th>Balance</th><th>Action</th></tr></table></div>")
            $.each(bills, function (i, v) {
                var balance = v.Statement.Balance - getTotalStatementPayments(v.Payments);
                if (balance <= 0) {
                    $('.billsTable').append("<tr><td>" + ConvertJsonDate(v.Statement.Date) + "</td><td>" + v.Orders[0].customer.LastName.toString() + " " + v.Orders[0].customer.FirstName.toString() + "</td><td>" + v.Statement.Id + "</td><td>" + parseFloat((v.Statement.Balance), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + "</td><td class=" + '"' + "balance" + '"' + ">" + parseFloat((balance), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + "</td><td><button class=" + '"' + "btn btn-info viewPdfBtn" + '"' + " style=" + '"' + "margin-right:5px" + '"' + " data-statementid=" + '"' + v.Statement.Id + '"' + ">View PDF</button></td></tr>");
                }
                else {
                    $('.billsTable').append("<tr><td>" + ConvertJsonDate(v.Statement.Date) + "</td><td>" + v.Orders[0].customer.LastName.toString() + " " + v.Orders[0].customer.FirstName.toString() + "</td><td>" + v.Statement.Id + "</td><td>" + parseFloat((v.Statement.Balance), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + "</td><td class=" + '"' + "balance" + '"' + ">" + parseFloat((balance), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + "</td><td><button class=" + '"' + "btn btn-info viewPdfBtn" + '"' + " style=" + '"' + "margin-right:5px" + '"' + " data-statementid=" + '"' + v.Statement.Id + '"' + ">View PDF</button><button class=" + '"' + "btn btn-success payStatementBtn" + '"' + " data-statementid=" + '"' + v.Statement.Id + '"' + " data-customerid=" + '"' + v.Orders[0].customer.Id + '"' + " data-balance=" + balance + ">Payment</button></td></tr>");
                }

            })
        })
    })

    $('#orderDiv').on('click', '.viewPdfBtn', function () {
        var statementId = $(this).data('statementid');
        $('#viewPdfIFrame').attr('src', "/home/GetPDF?statementId=" + statementId);
        $('#pdfModal').modal();
    })

    $('#orderDiv').on('click', '.payStatementBtn', function () {
        var rowIndex = $(this).closest('tr').index();
        var customerId = $(this).data('customerid');
        var orderId = $(this).data('statementid');
        var balance = $(this).data('balance');
        $('#sProcessPaymentBtn').attr('data-customerid', customerId);
        $('#sProcessPaymentBtn').attr('data-statementid', orderId);
        $('#sProcessPaymentBtn').attr('data-balance', balance);
        $('#sProcessPaymentBtn').data('customerid', customerId);
        $('#sProcessPaymentBtn').data('statementid', orderId);
        $('#sProcessPaymentBtn').data('balance', balance);
        $('#sProcessPaymentBtn').attr('row', rowIndex);
        $('#sDeductFromAcountBtn').attr('data-customerid', customerId);
        $('#sDeductFromAcountBtn').attr('data-statementid', orderId);
        $('#sDeductFromAcountBtn').attr('data-balance', balance);
        $('#sDeductFromAcountBtn').data('customerid', customerId);
        $('#sDeductFromAcountBtn').data('statementid', orderId);
        $('#sDeductFromAcountBtn').data('balance', balance);
        $('#sDeductFromAcountBtn').attr('row', rowIndex);
        $('#sPaymentModal').modal();

    })

    $('#sProcessPaymentBtn').on('click', function () {
        var customerId = $(this).data('customerid');
        var statementId = $(this).data('statementid');
        var rowIndex = $(this).attr('row');
        var balance = parseFloat($(this).data('balance'));
        var amount = parseFloat($('#amountPay').val());
        var note = $('#paymentNote').val();

        $.post("/home/AddStatementPayment", { customerId: customerId, statementId: statementId, amount: amount, paymentNote: note }, function () {
            $('#orderDiv tr:eq(' + rowIndex + ')').find('.payStatementBtn').data('balance', balance - amount);
            $('#orderDiv tr:eq(' + rowIndex + ') td:eq(4)').html(parseFloat(balance - amount, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString());
            $('#paymentAlertModal').modal();
            if (balance - amount <= 0) {
                $.post("/home/UpdateInvoicesPaid", { statementId: statementId }, function () {

                })
            }
        })

        $('#amountPay').val("");
        $('#paymentNote').val("");
    })

    $('#sDeductFromAcountBtn').on('click', function () {
        var customerId = $(this).data('customerid');
        var statementId = $(this).data('statementid');
        var rowIndex = $(this).attr('row');
        var balance = parseFloat($(this).data('balance'));
        var amount = parseFloat($('#amountPay').val());
        var note = $('#paymentNote').val();

        $.post("/home/StatementDeductFromAccount", { customerId: customerId, statementId: statementId, amount: amount, note: note }, function () {
            $('#orderDiv tr:eq(' + rowIndex + ')').find('.payStatementBtn').data('balance', balance - amount);
            $('#orderDiv tr:eq(' + rowIndex + ') td:eq(4)').html(parseFloat(balance - amount, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString());
        })
        $('#amountPay').val("");
        $('#paymentNote').val("");
    })

    $('#sFullAmountCheckbox').change(function () {
        if (this.checked) {
            var balance = $('#sProcessPaymentBtn').data('balance');
            balance = parseFloat(balance);
            $('#amountPay').val(balance)
        }
        else {
            $('#amountPay').val("");
        }
    })

    $('#allDates').change(function () {
        IsLoaderForInput = true;
        $('#filterDate').prop('disabled', function (i, v) { return !v; });
        var s = $('#searchHistoryInput').val().toString();
        var date = $('#filterDate').val();
        var opt = $('#filterOpt option:selected').val();
        var all = $('input[name=allDates]').is(':checked')
        $('.history').remove();
        $.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
            populateOrders(ordersHistory);
        })
    })

    $('#sPaymentModal').on('hidden.bs.modal', function () {
        $('#amountPay').val("");
        $('#sFullAmountCheckbox').attr('checked', false);
        $('#paymentNote').val("");
    })

    //$('.printAndEmail').on('click', function () {
    //    $(this).closest('tr').remove();
    //});

    function printStatement(statementId) {
        $('#pdf-iframe').attr("src", "/home/GetPDF?statementId=" + statementId).load(function () {
            document.getElementById('pdf-iframe').contentWindow.print();
        });
    }


    //$(".printAndEmail, .emailStatementBtn, .printStatement").on('click', function () {
    //    $(".printAndEmail, .emailStatementBtn, .printStatement").button('loading');
    //    setTimeout(function () {
    //        $(".printAndEmail, .emailStatementBtn, .printStatement").button('reset');
    //    }, 3000);
    //})



    //$('#trigger').on('click', function () {
    //    printTrigger($('#statementsPdf'));
    //});

    //$('#printStatementBtn').on('click', function () {
    //    printTrigger('statementPdf')
    //})

    //$('#p').on('click', function () {
    //    printTrigger('#iFramePdf');
    //})

    //function printTrigger(elementId) {
    //    var getMyFrame = document.getElementById($('#iFramePdf'));
    //    getMyFrame.focus();
    //    getMyFrame.contentWindow.print();
    //}
    $('#editProduct').on('click', function () {
        var productId = $(this).data('id');
    })

    $(".categoryBtn").on('click', function () {
        var c = $(this).data("category");
        $.post("/home/GetProductsByCategory", { categoryId: c }, function (products) {
            $(".productbtn").remove();
            products.forEach(function (product) {
                $.post("/home/GetProductAvailabilityByProductId", { productId: product.Id }, function (requestedAmount) {
                    var productName = product.ProductName.toString();
                    var availabeAmount = product.InStock - requestedAmount;
                    $("#productsInnerDiv").append("<button class=" + '"btn productbtn"' + " data-id=" + product.Id + " data-catererDiscount=" + product.CatererDiscount + " data-categoryId=" + product.CategoryId + " data-content=" + '"' + productName + '"' + " data-price=" + product.Price + " data-inStock=" + product.InStock + ">" + availabeAmount + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;$" + product.Price + "<br/ ><img src=" + "/Uploads/" + product.Image + "><br/>" + product.ProductName + "</button>")
                });
            })
        })
    })

    var catererTotal = 0;

    $("#productsInnerDiv").on('click', '.productbtn', function () {
        var exists = false;
        var p = $(this);
        var id = p.data("id");
        var productName = p.data("content");
        var inStock = p.data("instock");
        var price = p.data("price");
        var catererDiscount = p.data("catererdiscount");
        var categoryId = p.data("categoryid");
        var caterer = $('#catererDiscount').val();

        var row = "<tr data-price=" + price + " data-categoryid=" + categoryId + " data-id=" + id + " data-catererdiscount=" + catererDiscount + "><td><button class=" + '"btn btn-danger delete"' + ">X</button></td><td>" + productName + "</td><td><input class=" + '"input input-sm q"' + "v-model=" + '"quantity"' + " type=" + '"number"' + " value=" + '"1"' + "min=" + '"1"' + " /></td><td class=" + '"price"' + ">$" + price + "</td></tr>";
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

    //$('#applyChargesBtn').on('click', function () {
    //    var row = "<tr class="+'"charge"'+" data-price=" + price + " data-categoryid=" + categoryId + " data-id=" + id + " data-catererdiscount=" + catererDiscount + "><td><button class=" + '"btn btn-danger delete"' + ">X</button></td><td>" + productName + "</td><td><input class=" + '"input input-sm q"' + "v-model=" + '"quantity"' + " type=" + '"number"' + " value=" + '"1"' + "min=" + '"1"' + " /></td><td class=" + '"price"' + ">$" + price + "</td></tr>";
    //    $("#orderTable").append(row);

    //    RefreshOrder();
    //})

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
                    if (customer.Phone1 == null) {
                        customer.Phone1 = "";
                    }
                    if (customer.Phone2 == null) {
                        customer.Phone2 = "";
                    }
                    if (customer.Cell1 == null) {
                        customer.Cell1 = "";
                    }
                    if (customer.Cell2 == null) {
                        customer.Cell2 = "";
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


                    $('#searchTable').append("<tr class=" + '"customers"' + "><td>" + customer.LastName + " " + customer.FirstName + "</td><td>" + customer.Address + " " + customer.City + " " + customer.State + " " + customer.Zip + "</td><td>" + customer.Phone1 + " " + customer.Phone2 + "</td><td>" + customer.Cell1 + " " + customer.Cell2 + "</td><td><button class=" + '"' + "btn btn-info select" + '"' + " data-first=" + '"' + customer.FirstName + '"' + "  data-last=" + '"' + customer.LastName + '"' + "  data-add=" + '"' + customer.Address + '"' + "  data-phone1=" + '"' + customer.Phone1 + '"' + "  data-phone2=" + '"' + customer.Phone2 + '"' + " data-id=" + '"' + customer.Id + '"' + " data-cell1=" + '"' + customer.Cell1 + '"' + " data-cell2=" + '"' + customer.Cell2 + '"' + " data-caterer=" + '"' + customer.Caterer + '"' + " data-email=" + '"' + customer.Email + '"' + " >" + "Select" + "</button></td></tr>");
                })
            }
        })
    });

    $('#searchInput2').on('input', function () {
        $('.customers2').remove();
        $('#message2').remove();
        var s = $('#searchInput2').val().toString();
        $.post("/home/Search", { search: s }, function (customers) {
            if (s === "" || $.isEmptyObject(customers)) {
                //$('.customers').remove();
                $('#searchTable2').append("<h1 class=" + '"' + "message" + '"' + ">No matches found...</h1>");

            }
            else {
                customers.forEach(function (customer) {
                    if (customer.Phone1 == null) {
                        customer.Phone1 = "";
                    }
                    if (customer.Phone2 == null) {
                        customer.Phone2 = "";
                    }
                    if (customer.Cell1 == null) {
                        customer.Cell1 = "";
                    }
                    if (customer.Cell2 == null) {
                        customer.Cell2 = "";
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


                    $('#searchTable2').append("<tr class=" + '"customers2"' + "><td>" + customer.LastName + " " + customer.FirstName + "</td><td>" + customer.Address + " " + customer.City + " " + customer.State + " " + customer.Zip + "</td><td>" + customer.Phone1 + " " + customer.Phone2 + "</td><td>" + customer.Cell1 + " " + customer.Cell2 + "</td><td><button class=" + '"' + "btn btn-info select2" + '"' + " data-first=" + '"' + customer.FirstName + '"' + "  data-last=" + '"' + customer.LastName + '"' + "  data-add=" + '"' + customer.Address + '"' + "  data-phone1=" + '"' + customer.Phone1 + '"' + "  data-phone2=" + '"' + customer.Phone2 + '"' + " data-id=" + '"' + customer.Id + '"' + " data-cell1=" + '"' + customer.Cell1 + '"' + " data-cell2=" + '"' + customer.Cell2 + '"' + " data-caterer=" + '"' + customer.Caterer + '"' + " data-email=" + '"' + customer.Email + '"' + " data-city=" + '"' + customer.City + '"' + " data-state=" + '"' + customer.State + '"' + " data-zip=" + '"' + customer.Zip + '"' + " data-dismiss=" + '"' + "modal" + '"' + " >" + "Select" + "</button></td></tr>");
                })
            }
        })
    });

    $('#searchCustomerModal').on('click', '.select', function () {
        var fistName = $(this).data('first');
        var lastName = $(this).data('last');
        var address = $(this).data('add');
        var phone1 = $(this).data('phone1');
        var phone2 = $(this).data('phone2');
        var cell1 = $(this).data('cell1');
        var cell2 = $(this).data('cell2');
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
        $('#customerPhone').append("");
        $('#customerCell').append("");

        $('#customerHeader').append(fistName + " " + lastName);
        $('#customerAddress').append(address);
        $('#customerPhone').append(phone1 + " " + phone2);
        $('#customerCell').append(cell1 + " " + cell2);
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

    $('#searchRecipiantModal').on('click', '.select2', function () {
        $('#deliveryFirstName').val($(this).data('first'));
        $('#deliveryLastName').val($(this).data('last'));
        $('#deliveryAddress').val($(this).data('add'));
        $('#deliveryCity').val($(this).data('city'));
        $('#deliveryState').val($(this).data('state'));
        $('#deliveryZip').val($(this).data('zip'));
        $('#deliveryPhone1').val($(this).data('phone1'));
        $('#deliveryPhone2').val($(this).data('phone2'));
        $('#deliveryCell1').val($(this).data('cell1'));
        $('#deliveryCell2').val($(this).data('cell2'));
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

    //$('#searchHistoryInput').on('input', function () {
    //    IsLoaderForInput = true;
    //    var s = $('#searchHistoryInput').val().toString();
    //    var date = $('#filterDate').val();
    //    var opt = $('#filterOpt option:selected').val();
    //    var all = $('input[name=allDates]').is(':checked')
    //    $('.history').remove();
    //    $.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //        populateOrders(ordersHistory);
    //    })
    //})

    $('#searchHistory').on('click', function () {
        IsLoaderForInput = true;
        var s = $('#searchHistoryInput').val().toString();
        var date = $('#filterDate').val();
        var opt = $('#filterOpt option:selected').val();
        var all = $('input[name=allDates]').is(':checked')
        $('.history').remove();
        $.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
            if (ordersHistory.length <= 0) {
                $('#noMatches').html("No matches found!");
            }
            else {
                $('#noMatches').html("");
                populateOrders(ordersHistory);
            }
        })
    })

    $('#filterOpt').on('change', function () {
        IsLoaderForInput = true;
        var s = $('#searchHistoryInput').val().toString();
        var date = $('#filterDate').val();
        var opt = $('#filterOpt option:selected').val();
        var all = $('input[name=allDates]').is(':checked')
        $('.history').remove();
        $.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
            if (ordersHistory.length <= 0) {
                $('#noMatches').html("No matches found!");
            }
            else {
                $('#noMatches').html("");
                populateOrders(ordersHistory);
            }
        })
    })

    $('#filterDate').on('change', function () {
        IsLoaderForInput = true;
        var s = $('#searchHistoryInput').val().toString();
        var date = $('#filterDate').val();
        var opt = $('#filterOpt option:selected').val();
        var all = $('input[name=allDates]').is(':checked');
        $('.history').remove();
        $.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
            if (ordersHistory.length <= 0) {
                $('#noMatches').html("No matches found!");
            }
            else {
                $('#noMatches').html("");
                populateOrders(ordersHistory);
            }
        })
    })

    $('#edit').on('click', function () {
        location.href = $(this).attr('href');
    })

    $('#historyTable').on('click', '.viewDetailsBtn', function () {
        $("#table").find("tr:gt(0)").remove();
        $('#deliveryPanel').hide();
        $('#paymentDiv').html("");
        $('#odDeliveryInfo').html("");
        var ordersId = $(this).data('orderid');
        var customersId = $(this).data('customerid');
        var caterer = $(this).data('caterer');
        var subtotal = 0;
        var total = 0;
        var catererDiscount = 0;
        var stat = "";
        var deliveryOpt = $(this).data('deliveryopt');
        if (deliveryOpt == "Pickup") {
            $('#pickedupCheck').prop('checked', true);
            $('#deliveryCheck').prop('checked', false);
        }
        else if (deliveryOpt == "Delivery") {
            $('#deliveryCheck').prop('checked', true);
            $('#pickedupCheck').prop('checked', false);
        }
        if (caterer === true || caterer == "True") {
            $('#emailCheck').html("<label><div class=" + '"' + "form-group" + '"' + "><h2>Email invoice</h2><label class=" + '"' + "switch" + '"' + "><input type=" + '"' + "checkbox" + '"' + "checked=" + '"' + "checked" + '"' + " id=" + '"' + "emailConfirm" + '"' + "><div class=" + '"' + "slider round" + '"' + "></div></label></div></label>");
        }
        else {
            $('#emailCheck').html("<label><div class=" + '"' + "form-group" + '"' + "><h2>Email invoice</h2><label class=" + '"' + "switch" + '"' + "><input type=" + '"' + "checkbox" + '"' + " id=" + '"' + "emailConfirm" + '"' + "><div class=" + '"' + "slider round" + '"' + "></div></label></div></label>");
        }
        $('#edit').attr('href', "/home/editOrder?customerId=" + customersId + "&orderId=" + ordersId);
        $('#cancel').attr('data-id', ordersId);
        $('#updateStatusBtn').attr('data-orderid', ordersId);
        $('#updateStatusBtn').attr('data-customerid', customersId);
        $('#updateStatusBtn').data('orderid', ordersId);
        $('#updateStatusBtn').data('customerid', customersId);
        $('#emailInvoice').attr('data-orderid', ordersId);
        //$('#edit').attr('data-orderid', ordersId);

        $.post("/home/GetOrderStatus", { orderId: ordersId }, function (status) {
            stat = status.Status1;
            if (stat === "Delivered" || stat === "Picked up") {
                //$('#uncomplete').show();
                $('#edit').prop("disabled", true);
                $('#cancel').prop("disabled", true);
                //$('#statusBtn').prop("disabled", true);
                $('#statusBtn').html("Open Order");
                $('#statusBtn').attr("id", "openOrder");
            }
            else {
                //$('#uncomplete').hide();
                $('#statusBtn').html("Update Status");
                $('#statusBtn').attr("id", "updateStatus");
            }
        })
        $.post("/home/GetOrderHistory", { customerId: ordersId, orderId: customersId }, function (ordersHistory) {
            if (ordersHistory.order.DeliveryOption === "Delivery") {
                $('#deliveryPanel').show();
                $('#odDeliveryInfo').html("<h4>" + ordersHistory.order.DeliveryFirstName + " " + ordersHistory.order.DeliveryLastName + "</h4><h4>" + ordersHistory.order.DeliveryAddress + "</h4><h4>" + ordersHistory.order.DeliveryCity + " " + ordersHistory.order.DeliveryState + " " + ordersHistory.order.DeliveryZip + "</h4><h4>" + ordersHistory.order.Phone1 + " " + ordersHistory.order.Phone2 + "</h4><h4>" + ordersHistory.order.Cell1 + " " + ordersHistory.order.Cell2 + "</h4>");
                $('#deliveryCharge').html("Delivery Charge: " + parseFloat(ordersHistory.order.DeliveryCharge, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString());
                catererDiscount = ordersHistory.order.DeliveryCharge;
            }
            else {
                $('#deliveryCharge').html("");
            }
            if (ordersHistory.payments != null) {
                ordersHistory.payments.forEach(function (p) {
                    var date = ConvertJsonDate(p.Date);
                    var note = "(" + p.PaymentNote + ")";
                    if (p.PaymentNote == "") {
                        note = "";
                    }
                    $('#paymentsBody').append("<h5 class=" + '"' + "payments" + '"' + " >Payment of: $" + p.Payment1 + " on " + date + " paid with " + p.PaymentMethod + " " + note + "</h5>");
                    //$('#paymentDiv').append("<h5>Payment of: $" + p.Payment1 + " on " + date + " paid with " + p.PaymentMethod + " " + note + "</h5>");
                })
            }
            var totalExludingDis = 0;
            var discount = ordersHistory.order.Discount;

            $('#notesBody').html("<h5>" + ordersHistory.order.Notes + "</h5>");
            $('#greetingsBody').html("<h5>" + ordersHistory.order.Greetings + "</h5>")
            $('#odCustomerInfo').html("<h4>" + ordersHistory.customer.FirstName + " " + ordersHistory.customer.LastName + "</h4><h4>" + ordersHistory.customer.Address + "</h4><h4>" + ordersHistory.customer.City + " " + ordersHistory.customer.State + " " + ordersHistory.customer.Zip + "</h4><h4>Phone: " + ordersHistory.customer.Phone1 + " " + ordersHistory.customer.Phone2 + "</h4><h4>Cell: " + ordersHistory.customer.Cell1 + " " + ordersHistory.customer.Cell2 + "</h4><h5>" + ordersHistory.customer.Email + "</h5>");
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
                    totalExludingDis += t;
                    if (orderedProducts.catererDiscount < 1) {
                        catererDiscount = t * orderedProducts.catererDiscount;
                        total += (t - catererDiscount);
                    }
                    else {
                        catererDiscount = orderedProducts.catererDiscount;
                        total += (t - catererDiscount * orderedProducts.quantity);
                    }

                    //}
                    //else if (orderedProducts.categoryId === 2 || orderedProducts.categoryId === 3) {
                    //    catererDiscount = t * 0.1;
                    //    total += (t - catererDiscount);

                    //}
                    //else if (orderedProducts.categoryId === 4) {
                    //    catererDiscount = 2.5;
                    //    total += (t - catererDiscount * orderedProducts.quantity);

                    //}
                    //else {
                    //    total += t;
                    //}
                    if (orderedProducts.caterer === true || caterer === "True" && (orderedProducts.catererDiscount < 1)) {
                        catererHtml = (t + "<span style=" + '"color: red"' + "> (-" + parseFloat(catererDiscount, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + ")</span>");
                    }
                    else {
                        catererHtml = (t + "<span style=" + '"color: red"' + "> (-" + parseFloat(catererDiscount * orderedProducts.quantity, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + ")</span>");
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
            if (discount < 1) {
                discount = discount * total;
            }
            if (caterer === "True") {
                $('#odDiscount').html("Discount: " + parseFloat(totalExludingDis - total + discount, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString());
                $('#odSubtotal').html("Subtotal: $" + parseFloat(totalExludingDis, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString());
            }
            else {
                $('#odDiscount').html("Discount: " + parseFloat(discount, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString());
                $('#odSubtotal').html("Subtotal: $" + parseFloat(total, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString());
            }
            //if (discount >= 1) {

            $('#odTotal').html("Total: $" + parseFloat((total + catererDiscount - discount), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString());
            //}
            //else {
            //$('#odTotal').html("Total: $" + parseFloat((total - (total * discount)), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString());
            //}
        })
        $('#orderDetailsModal').modal('show');
    })


    $('#orderDetailsModal').on('hidden.bs.modal', function () {
        $('#edit').prop("disabled", false);
        $('#cancel').prop("disabled", false);
        $('#statusBtn').prop("disabled", false);
        $('.payments').remove();
    })
    $('#inventoryBtn').on('click', function () {
        var productId = $(this).data('productid')
        var productName = $(this).data('productName')
        $('#productNameInv').html(productName);
        $('#quantityInv').val()
    })

    $('.add').on('click', function () {
        var element = $(this)
        var id = $(this).data('id');
        var amount = parseInt($(this).closest('tr').find('.invQuantityInput').val());
        var inStock = parseInt(element.closest('tr').find('.inStock').html());
        var avail = parseInt(element.closest('tr').find('.badgeDiv span').html());
        $.post("/Home/UpdateInventory", { id: id, amount: amount }, function () {
            element.closest('tr').find('.inStock').html(inStock += amount);
            element.closest('tr').find('.badgeDiv span').html(avail += amount);
            element.closest('tr').find('.invQuantityInput').val("");
        })
    })

    $('.productEditBtn').on('click', function () {
        var productId = $(this).data("id");
        $('#productId').val(productId);

        $("#categories option").remove();

        $.post("/Home/GetCatagories", function (categories) {
            for (i = 0; i < categories.length; i++) {
                $('#categories').append(" <option value=" + '"' + categories[i].Id + '"' + ">" + categories[i].CategoryName + "</option>")
            }
        })
        $.post("/Home/GetProduct", { productId: productId }, function (product) {
            $('#submitProdChanges').attr('data-id', productId);
            $('#productHeader').html(product.ProductName);
            //var category = $('#categories').find('option').val();
            $('#productName').val(product.ProductName);
            $('#categories').val(product.CategoryId);
            $('#price').val(product.Price);
            $('#catererDiscount').val(product.CatererDiscount);
            $('#restockAmount').val(product.RestockAmount);
            $('#inStock').val(product.InStock);
            $('#imageDiv').html("<img src=" + '"' + "/Uploads/" + product.Image + '"' + " />");
            $('#sortIndex').val(product.SortIndex);
            $('#discontinued').prop('checked', product.Discontinued);
            $('#editProductModal').modal();
        })
    })


    // ----------------------------------------------------------Product Edit---------------------------
    //$('#submitProdChanges').on('click', function () {
    //    var productId = parseInt($(this).attr('data-id'));
    //    var categoryId = parseInt($('#categories option:selected').val());
    //    var productName = $('#productName').val();
    //    var price = parseFloat($('#price').val());
    //    var catererDiscount = parseFloat($('#catererDiscount').val());
    //    var restockAmount = parseInt($('#restockAmount').val());
    //    var inStock = parseInt($('#inStock').val());
    //    var image = $('#image').val();
    //    var discontinued = $('#discontinued').val();
    //    $.post("/Home/EditProduct", { productId: productId, categoryId: categoryId, productName: productName, price: price, catererDiscount: catererDiscount, restockAmount: restockAmount, inStock: inStock, image: image, discontinued: discontinued }, function () {
    //        alert("Changes where applied!");
    //    })
    //})

    //$('#submitProdChanges').on("hidden.bs.modal", function () {
    //    $('#categories').remove('option');
    //    $('#productName').val("");
    //    $('#price').val("");
    //    $('#catererDiscount').val("");
    //    $('#restockAmount').val("");
    //    $('#inStock').val("");
    //    $('#image').val("");
    //    $('#discontinued').val("");
    //})

    //-------------------------------------------------------------------------------------------------------------

    $('#filterDeliveryDate, #deliveryOpt, #filterByCity').on('change', function () {
        IsLoaderForInput = true;
        var x = $('#filterDeliveryDate option:selected').val();
        var filterByCity = $('#filterByCity option:selected').val();
        var deliveryOpt = $('#deliveryOpt option:selected').val();
        var pickup = false;
        $('.deliveryInfoDiv').remove();
        $('#deliveryAlert').remove();
        $.post("/Home/DeliveryFilter", { x: x, deliveryOpt: deliveryOpt }, function (deliveries) {
            if (deliveryOpt == "Delivery") {
                $('#deliveryHeader').css("color", "orange");
            }
            else {
                pickup = true;
                $('#deliveryHeader').css("color", "blue");
            }
            $('#deliveryHeader').html(deliveryOpt + "'s");
            if (filterByCity !== "All") {
                deliveries = deliveries.filter(d => d.order.DeliveryCity.toLowerCase() === filterByCity.toLowerCase());
            }
            console.log("filter");
            populateDeliveries(deliveries, pickup);
        })
    });

    $('#filterByCity').on('change', function () {
        IsLoaderForInput = true;
        var x = $('#filterByCity option:selected').val();
        var deliveryOpt = $('#deliveryOpt option:selected').val();
        var pickup = false;
        $('.deliveryInfoDiv').remove();
        $('#deliveryAlert').remove();
        $.post("/Home/DeliveryFilter", { x: x, deliveryOpt: deliveryOpt }, function (deliveries) {
            if (deliveryOpt == "Delivery") {
                $('#deliveryHeader').css("color", "orange");
            }
            else {
                pickup = true;
                $('#deliveryHeader').css("color", "blue");
            }
            $('#deliveryHeader').html(deliveryOpt + "'s");
            populateDeliveries(deliveries, pickup);
        })
    });

    function populateDeliveries(deliveries, pickup) {
        if ($.isEmptyObject(deliveries)) {
            $('#deliveryAlertDiv').append("<h3 id=" + '"' + "deliveryAlert" + '"' + ">No deliveries for this time period...</h3>");
        }
        for (var i = 0, l = deliveries.length; i < l; i++) {
            var requiredDate = ConvertJsonDate(deliveries[i].order.RequiredDate);
            var firstName = deliveries[i].customer.FirstName;
            var lastName = deliveries[i].customer.LastName;
            var phone1 = deliveries[i].customer.Phone1;
            var phone2 = deliveries[i].customer.Phone2;
            var cell1 = deliveries[i].customer.Cell1;
            var cell2 = deliveries[i].customer.Cell2;
            var deliveryFirstName = deliveries[i].order.DeliveryFirstName;
            var deliveryLastName = deliveries[i].order.DeliveryLastName;
            var deliveryAddress = deliveries[i].order.DeliveryAddress;
            var deliveryCity = deliveries[i].order.DeliveryCity;
            var deliveryState = deliveries[i].order.DeliveryState;
            var deliveryZip = deliveries[i].order.DeliveryZip;
            var deliveryPhone1 = deliveries[i].order.Phone1;
            var deliveryPhone2 = deliveries[i].order.Phone2;
            var deliveryCell1 = deliveries[i].order.Cell1;
            var deliveryCell2 = deliveries[i].order.Cell2;
            var deliveryNote = deliveries[i].order.DeliveryNote;
            var total = deliveries[i].total;
            var payments = deliveries[i].payments;
            var greetings = deliveries[i].order.Greetings;
            var note = deliveries[i].order.Notes;

            for (var j = 0, k = payments.length; j < k; j++) {
                total -= payments[j].Payment1;
            }


            var productsHtml = "";
            var quantity = "";
            var productName = "";

            if ($.isEmptyObject(deliveries[i].orderedProducts)) {
                productsHtml = "No products for this order..."
            }
            else {
                for (var x = 0, d = deliveries[i].orderedProducts.length; x < d; x++) {
                    quantity = "";
                    if (deliveries[i].orderedProducts != null) {
                        quantity = deliveries[i].orderedProducts[x].quantity;
                    }
                    productName = "";
                    if (deliveries[i].orderedProducts != null) {
                        productName = deliveries[i].orderedProducts[x].productName;
                    }
                    productsHtml += "<h5>" + quantity + " - " + productName + "</h5>";
                }
            }
            if (pickup) {
                $('#deliveriesDiv').append("<div class=" + '"' + "deliveryInfoDiv" + '"' + "><div class=" + '"' + "panel panel-info deliveryInnerDiv" + '"' + "><div class=" + '"' + "panel-heading" + '"' + ">" + requiredDate + " " + firstName + " " + lastName + "&nbsp&nbsp&nbsp&nbsp" + phone1 + "&nbsp&nbsp&nbsp&nbsp" + phone2 + "&nbsp&nbsp&nbsp&nbsp" + cell1 + "&nbsp&nbsp&nbsp&nbsp" + cell2 + " <span style=" + '"' + "float:right" + '"' + " >$" + total + "</span></div><div class=" + '"' + "panel-body body" + '"' + "><div class=" + '"' + "deliveryLeftDiv" + '"' + ">" + productsHtml + "</div><div class=" + '"' + "deliveryMiddleDiv" + '"' + "><div class=" + '"' + "form-group" + '"' + "><label for=" + '"' + "greetings" + '"' + ">Greetings:</label><p id=" + '"' + "greetings" + '"' + ">" + greetings + "</p><label for=" + '"' + "note" + '"' + ">Note:</label><p id=" + '"' + "note" + '"' + ">" + note + "</p></div></div><div class=" + '"' + "deliveryRightDiv" + '"' + "><h4>" + paymentMethod + "</h4></div></div></div></div></div>");
            }
            else {
                $('#deliveriesDiv').append("<div class=" + '"' + "deliveryInfoDiv" + '"' + "><div class=" + '"' + "panel panel-info deliveryInnerDiv" + '"' + "><div class=" + '"' + "panel-heading" + '"' + ">" + requiredDate + " " + firstName + " " + lastName + "&nbsp&nbsp&nbsp&nbsp" + phone1 + "&nbsp&nbsp&nbsp&nbsp" + phone2 + "&nbsp&nbsp&nbsp&nbsp" + cell1 + "&nbsp&nbsp&nbsp&nbsp" + cell2 + " <span style=" + '"' + "float:right" + '"' + " >$" + total + "</span></div><div class=" + '"' + "panel-body body" + '"' + "><div class=" + '"' + "deliveryLeftDiv" + '"' + "><h4>" + deliveryFirstName + " " + deliveryLastName + "<br />" + deliveryAddress + "<br />" + deliveryCity + " " + deliveryState + " " + deliveryZip + "<br /><br />" + deliveryPhone1 + " " + deliveryPhone2 + "<br />" + deliveryCell1 + " " + deliveryCell2 + "</h4></div><div class=" + '"' + "deliveryMiddleDiv" + '"' + ">" + productsHtml + "</div><div class=" + '"' + "deliveryRightDiv" + '"' + "><div class=" + '"' + "form-group" + '"' + "><label for=" + '"' + "deliverynote" + '"' + ">Delivery Note:</label><p id=" + '"' + "deliveryNote" + '"' + ">" + deliveryNote + "</p><label for=" + '"' + "greetings" + '"' + ">Greetings:</label><p id=" + '"' + "greetings" + '"' + ">" + greetings + "</p><label for=" + '"' + "note" + '"' + ">Note:</label><p id=" + '"' + "note" + '"' + ">" + note + "</p></div></div></div></div></div>");
            }
        }
    }


    function populateOrders(ordersHistory) {
        if (ordersHistory.length === 0) {
            $('#ordersAlert').html("No orders for this time period...");
        }
        else {
            $('#ordersAlert').html("");
        }
        for (var i = 0, l = ordersHistory.length; i < l; i++) {
            var paidHtml = "<td></td>";
            var deliveryHtml = "";
            var requiredDate = ConvertJsonDate(ordersHistory[i].requiredDate);
            var discount = ordersHistory[i].discount;
            var invoice = ordersHistory[i].invoice;
            var id = ordersHistory[i].id;
            var deliveryOption = ordersHistory[i].deliveryOpt;
            var firstName = ordersHistory[i].firstName;
            var lastName = ordersHistory[i].lastName;
            var customerId = ordersHistory[i].customerId;
            var payments = ordersHistory[i].payments;
            var caterer = ordersHistory[i].caterer;
            var status = "";
            var paymentMethod = ordersHistory[i].paymentMethod;
            if (ordersHistory[i].status != null) {
                status = ordersHistory[i].status.Status1;
            }
            var p = 0;
            var total = 0;
            var orderTotal = getTotal(id, customerId);
            //if (discount < 1) {
            //    discount = (orderTotal * discount);
            //    total = parseFloat((orderTotal - discount), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
            //}
            //else {
            //    total = parseFloat((orderTotal - discount), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
            //}
            total = parseFloat((orderTotal), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();

            payments.forEach(function (payment) {
                p += payment.Payment1;
            })
            var balance = parseFloat(orderTotal - p, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString();
            var b = orderTotal - p;
            //var total = 0;
            if (ordersHistory[i].paymentMethod == "On Account") {
                paidHtml = "<td class=" + '"balance"' + "><span style=" + '"color:green"' + ">On Account </span></td>";
            }
            else if (ordersHistory[i].statement) {
                paidHtml = "<td class=" + '"balance"' + "><span style=" + '"color:blue"' + ">Billed </span></td>";
            }
            else if (ordersHistory[i].paid || balance <= 0) {
                paidHtml = "<td class=" + '"balance"' + "><span style=" + '"color:green"' + ">Paid </span><span style=" + '"color:green"' + " class=" + '"glyphicon glyphicon-ok"' + "></span></td>";
            }
            else {
                paidHtml = "<td class=" + '"balance"' + "><span style=" + '"color: red"' + ">" + balance + "</span></td>";
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
            var paymentBtnHtml = "";
            if (ordersHistory[i].paid || ordersHistory[i].statement || ordersHistory[i].paymentMethod == "On Account") {
                paymentBtnHtml = "<button class=" + '"btn btn-success paymentBtn"' + "data-orderid=" + id + " data-customerid=" + customerId + " data-balance=" + b + " disabled >Payment</button>";
            }
            else {
                paymentBtnHtml = "<button class=" + '"btn btn-success paymentBtn"' + "data-orderid=" + id + " data-customerid=" + customerId + " data-balance=" + b + ">Payment</button>";
            }
            var viewInvoiceHtml = "";
            if (ordersHistory[i].invoice) {
                viewInvoiceHtml = "<button class=" + '"btn btn-default viewInvoiceBtn"' + " data-orderid=" + id + " data-customerid=" + customerId + ">Invoice</button>"
            }
            else {
                viewInvoiceHtml = "<button class=" + '"btn btn-default viewInvoiceBtn"' + "disabled data-orderid=" + id + " data-customerid=" + customerId + ">Invoice</button>"
            }
            var closeOrderHtml = "";
            if (!ordersHistory[i].invoice) {
                closeOrderHtml = "<button class=" + '"' + "btn btn-primary updateStatusBtn" + '"' + " data-orderid=" + '"' + id + '"' + " data-customerid=" + '"' + customerId + '"' + ">Complete</button>";
            }
            else {
                closeOrderHtml = "<button class=" + '"' + "btn btn-primary updateStatusBtn" + '"' + " data-orderid=" + '"' + id + '"' + " data-customerid=" + '"' + customerId + '"' + " disabled>Completed</button>";
            }
            $('#historyTable').append("<tr class=" + '"history"' + "><td>" + lastName + " " + firstName + "</td><td>" + requiredDate + "</td><td>" + deliveryOption + deliveryHtml + "</td><td>" + total + "</td><td>" + paymentMethod + "</td><td class=" + '"' + "status" + '"' + ">" + status + "</td>" + paidHtml + " <td><button class=" + '"btn btn-info viewDetailsBtn"' + "data-deliveryopt=" + '"' + deliveryOption + '"' + "data-orderid=" + '"' + id + '"' + "data-customerid=" + '"' + customerId + '"' + "data-caterer=" + '"' + caterer + '"' + ">Details</button>" + paymentBtnHtml + "<br/>" + viewInvoiceHtml + closeOrderHtml + "</td></tr>");
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

    $('#selectRecipiant').on('click', function () {
        $('#searchRecipiantModal').modal();
    })

    $('#newCustomerSubmit').on('click', function () {
        if ($('#phone1').val() == "" && $('#cell1').val() == "") {
            $('#alertInvalidSubmit').modal();
        }
        else {
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
            var phone1 = $('#phone1').val();
            var phone2 = $('#phone2').val();
            var cell1 = $('#cell1').val();
            var cell2 = $('#cell2').val();
            var email = $('#email').val();
            $.post("/home/AddNewCustomer", { firstName: firstName, lastName: lastName, address: address, city: city, state: state, zip: zip, phone1: phone1, phone2: phone2, cell1: cell1, cell2: cell2, caterer: caterer, email: email }, function (id) {
                $('#customerId').val(id);
                $('#customerIdCheckout').val(id);
                $('#customerHeader').text("");
                $('#customerAddress').text("");
                $('#customerPhone').text("");
                $('#customerCell').text("");
                $('#discountInput').val("");
                $('#catererIndicator').html("");
                $('#customerEmail').text("");

                $('#customerHeader').append(firstName + " " + lastName);
                $('#customerAddress').append(address);
                $('#customerPhone').append(phone1 + " " + phone2);
                $('#customerCell').append(cell1 + " " + cell2);
                $('.customers').remove();
                $('#searchInput').val("");
                $('#catererDiscount').val(caterer);
                if (caterer == true) {
                    $('#catererIndicator').html("<span style=" + '"color:blue"' + ">*Caterer*</span>");
                }
                $('#customerEmail').append(email);
                $('#alertNewCustomerAdded').modal();
            })
        }
    })



    $('#alertNewCustomerOk').on('click', function () {
        $('#searchCustomerModal').modal('toggle');
        $('#newCustomerModal').modal('toggle');
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

    $('#addCharge').on('click', function () {
        $('#addChargeModal').modal();
    })


    $('#historyTable').on('click', '.paymentBtn', function () {
        elem = $(this);
        var customerId = $(this).data('customerid');
        var orderId = $(this).data('orderid');
        var balance = $(this).data('balance');
        $('#processPaymentBtn').attr('data-customerid', customerId);
        $('#processPaymentBtn').attr('data-orderid', orderId);
        $('#processPaymentBtn').attr('data-balance', balance);
        $('#processPaymentBtn').data('customerid', customerId);
        $('#processPaymentBtn').data('orderid', orderId);
        $('#processPaymentBtn').data('balance', balance);
        $('#paymentModal').modal();
    })

    $('#paymentModal').on("hidden.bs.modal", function () {
        $('#fullAmountCheckbox').attr('checked', false);
        $('#amountPay').val("");
        $('#paymentNote').val("");
    })

    $('#placeOrderBtn').on('click', function () {
        if ($('#customerIdCheckout').val() === "") {
            $('#alertInvalidCustomer').modal();
        }
        else if ($('#orderTable tr').length < 2) {
            $('#alertInvalidOrder').modal();
        }
        else {
            $('#checkoutModal').modal();
        }
    })

    $('#processPaymentBtn').on('click', function () {
        var customerId = $(this).data('customerid');
        var balance = $(this).data('balance');
        var orderId = $(this).data('orderid');
        var amount = $('#amountPay').val();
        var note = $('#paymentNote').val();
        var paymentMethod = $('input[name=paymentMethod]:checked').val();

        $.post("/home/MakePayment", { customerId: customerId, orderId: orderId, amount: amount, note: note, paymentMethod: paymentMethod }, function () {
            //$('#paymentAlertModal').modal();
            //var s = $('#searchHistoryInput').val().toString();
            //var date = $('#filterDate').val();
            //var opt = $('#filterOpt option:selected').val();
            //var all = $('input[name=allDates]').is(':checked');
            //$('.history').remove();
            //IsLoaderForInput = true;
            //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
            //    populateOrders(ordersHistory);
            //})
            var prevBalance = parseFloat(balance);
            var curBalance = prevBalance -= amount;
            //$(this).attr('data-balance', curBalance);
            $(this).data('balance', curBalance);
            if (curBalance <= 0) {
                elem.closest('tr').find('.balance').html("<span style=" + '"color: green"' + ">Paid</span><span style=" + '"color:green"' + " class=" + '"glyphicon glyphicon-ok"' + "></span></td>");
                elem.closest('.paymentBtn').prop('disabled', true);
            }
            else {
                elem.closest('tr').find('.balance').html("<span style=" + '"color: red"' + ">" + parseFloat(curBalance, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString() + "</span></td>");
            }
            elem.closest('.paymentBtn').data('balance', curBalance);
            elem = "";
        })

        $('#amountPay').val("");
        $('#paymentNote').val("");
    })

    //$('#paymentAlertModal').on('hidden.bs.modal', function () {
    //    location.reload();
    //})

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
                $('#deliveryPhone1').val(customer.Phone1)
                $('#deliveryPhone2').val(customer.Phone2)
                $('#deliveryCell1').val(customer.Cell1)
                $('#deliveryCell2').val(customer.Cell2)
            })
        }
        else {
            $('#deliveryFirstName').val("")
            $('#deliveryLastName').val("")
            $('#deliveryAddress').val("")
            $('#deliveryCity').val("")
            $('#deliveryState').val("")
            $('#deliveryZip').val("")
            $('#deliveryPhone1').val("")
            $('#deliveryPhone2').val("")
            $('#deliveryCell1').val("")
            $('#deliveryCell2').val("")
        }
    });

    $('#cancel').on('click', function () {
        var id = $(this).data('id');
        $('#cancelYesBtn').attr('id', id);

        $('#cancelAlertModal').modal();
        //$.post("/home/DeleteOrder", { id: id }, function () {
        //    window.location.href = ('/home/orderHistory');
        //    alert("Order #" + id + " was cancelled!");
        //})
    })

    $('#cancelYesBtn').on('click', function () {
        var id = $(this).attr('id');
        $.post("/home/DeleteOrder", { id: id }, function () {
            $('#cancelStatusModal').modal();
        })
    });

    $('#cancelStatusModal').on('hidden.bs.modal', function () {
        window.location.href = ('/home/orderHistory');
    });

    $('#emailInvoiceBtn').on('click', function () {
        orderId = $(this).data('orderid');
        customerId = $(this).data('customerid');
        $.post("/home/GetCustomerById", { id: customerId }, function (customer) {
            if (customer.Caterer && customer.Email != "" && customer.Email != null) {
                $.post("/home/EmailInvoice", { customerId: customerId, orderId: orderId }, function () {
                    $('#invoiceAlertMessage').html("An invoice was sent via email to " + customer.FirstName + " " + customer.LastName + " !");
                    $('#invoiceAlertModal').modal();
                })
            }
            else {
                $('#invoiceAlertMessage').html("<span style=" + '"' + "color:red" + '"' + ">" + customer.FirstName + " " + customer.LastName + " does not have an email address on file...</span>");
                $('#invoiceAlertModal').modal();
            }
        })
    })

    $('#historyTable').on('click', '.updateStatusBtn', function () {
        t = $(this);
        oId = t.data('orderid');
        $('#completeYesBtn').attr('data-orderid', oId);
        $('#completeNoBtn').attr('data-orderid', oId);
        //var customer = getCustomer(t.data('customerid'));
        $.post("/home/GetCustomerById", { id: t.data('customerid') }, function (customer) {
            cust = customer;
            cId = customer.Id;
            if (customer.Email != "" && customer.Email != null) {
                $('#updateConfirmAlert').html("Do you want to send " + customer.FirstName + " " + customer.LastName + " a reciept to " + customer.Email + "?");
                $('#completeYesBtn').html("Yes");
                $('#completeNoBtn').show();
            }
            else {
                $('#updateConfirmAlert').html(customer.FirstName + " " + customer.LastName + " does not have an email address on file!");
                $('#completeYesBtn').html("Continue");
                $('#completeNoBtn').hide();
            }
            $('#updateConfirmation').modal();
        })
    })

    $('#completeNobtn').on('click', function () {
        t.attr('disabled', true);
        t.html("Completed");
        var status = "";
        $.post("/home/GetOrderById", { orderId: $('#completeYesBtn').attr('data-orderid') }, function (order) {
            if (order.Status == "Delivery") {
                status = "Delivered";
            }
            else {
                status = "Picked up";
            }
            t.closest('tr').find('.status').html(status);
            $.post("/home/UpdateStatus", { orderId: $('#completeYesBtn').attr('data-orderid'), status: status }, function () {
                //location.reload();
            })
        })
        $.post("/home/CreateInvoice", { customerId: cId, orderId: oId }, function () {
            $('#invoiceAlertMessage').html("Invoice generated successfuly!");
            $('#invoiceAlertModal').modal();
            //$('#orderDetailsModal').modal('toggle');
            var s = $('#searchHistoryInput').val().toString();
            var date = $('#filterDate').val();
            var opt = $('#filterOpt option:selected').val();
            var all = $('input[name=allDates]').is(':checked');
            //$('.history').remove();
            //IsLoaderForInput = true;
            //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
            //    populateOrders(ordersHistory);
            //    $('#invoiceAlertModal').modal();
            //})
            t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
            return false;
        })
    })

    $('#completeYesBtn').on('click', function () {
        t.attr('disabled', true);
        t.html("Completed");
        var status = "";
        $.post("/home/GetOrderById", { orderId: $('#completeYesBtn').attr('data-orderid') }, function (order) {
            if (order.Status == "Delivery") {
                status = "Delivered";
            }
            else {
                status = "Picked up";
            }
            t.closest('tr').find('.status').html(status);
            $.post("/home/UpdateStatus", { orderId: $('#completeYesBtn').attr('data-orderid'), status: status }, function () {
                //location.reload();
            })
        })
        if ($('#emailInput').val() != "") {
            var email = $('#emailInput').val();
            $('#emailInput').val("");
            if ($('#saveEmail').is(':checked')) {
                $('#saveEmail').prop('checked', false);
                $.post("/home/SaveEmail", { customerId: cId, email: email }, function () {
                })
            }
            $.post("/home/CreateInvoiceOtherEmail", { customerId: cId, orderId: $('#completeYesBtn').attr('data-orderid'), email: email }, function () {
                $('#invoiceAlertMessage').html('An invoice was sent to the email address you provided!');
                //$('#orderDetailsModal').modal('toggle');
                $('#invoiceAlertModal').modal();
                var s = $('#searchHistoryInput').val().toString();
                var date = $('#filterDate').val();
                var opt = $('#filterOpt option:selected').val();
                var all = $('input[name=allDates]').is(':checked');
                //$('.history').remove();
                //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
                //    IsLoaderForInput = true;
                //    populateOrders(ordersHistory);
                //    $('#invoiceAlertModal').modal();
                //})
                t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
            })
        }
        else {
            if (cust.Email != "" && cust.Email != null) {
                $.post("/home/CreateInvoiceEmail", { customerId: cId, orderId: $('#completeYesBtn').attr('data-orderid') }, function () {
                    $('#invoiceAlertMessage').html("An invoice was sent via email to " + cust.FirstName + " " + cust.LastName + "!");
                    //$('#orderDetailsModal').modal('toggle');
                    $('#invoiceAlertModal').modal();
                    var s = $('#searchHistoryInput').val().toString();
                    var date = $('#filterDate').val();
                    var opt = $('#filterOpt option:selected').val();
                    var all = $('input[name=allDates]').is(':checked');
                    //$('.history').remove();
                    //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
                    //    IsLoaderForInput = true;
                    //    populateOrders(ordersHistory);
                    //    $('#invoiceAlertModal').modal();
                    //})
                    t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
                })
            }
            //else if (customer.Email == null && customer.Caterer == true) {
            //    $.post("/home/CreateInvoice", { customerId: cId, orderId: $('#completeYesBtn').attr('data-orderId') }, function () {
            //        $('#invoiceAlertMessage').html("Invoice generated successfuly!");
            //        $('#invoiceAlertModal').modal();
            //        //$('#orderDetailsModal').modal('toggle');
            //        var s = $('#searchHistoryInput').val().toString();
            //        var date = $('#filterDate').val();
            //        var opt = $('#filterOpt option:selected').val();
            //        var all = $('input[name=allDates]').is(':checked');
            //        //$('.history').remove();
            //        //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
            //        //    populateOrders(ordersHistory);
            //        //})
            //        t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
            //    })
            //}
        }
    })

    function getCustomer(customerId) {
        $.post("/home/GetCustomerById", { id: customerId }, function (customer) {
            return customer;
        })
    }

    //$('#historyTable').on('click', '.updateStatusBtn', function () {
    //    var t = $(this);
    //    var orderId = t.data('orderid');
    //    var customerId = t.data('customerid');
    //    var status = "";
    //    var email = $('#emailInput').val();
    //    $('#completeYesBtn').attr('data-orderId', orderId);
    //    $('#completeNoBtn').attr('data-orderId', orderId);
    //    $.post("/home/GetCustomerById", { id: customerId }, function (customer) {
    //        var customer = customer;
    //        if (customer.Email != "" && customer.Email != null) {
    //            $('#updateConfirmAlert').html("Do you want to send " + customer.FirstName + " " + customer.LastName + " a reciept to " + customer.Email + "?");
    //            $('#completeYesBtn').html("Yes");
    //            $('#completeNoBtn').show();
    //        }
    //        else {
    //            $('#updateConfirmAlert').html(customer.FirstName + " " + customer.LastName + " does not have an email address on file!");
    //            $('#completeYesBtn').html("Continue");
    //            $('#completeNoBtn').hide();
    //        }
    //        if (status == undefined) {
    //            status = "";
    //        }
    //        $('#updateConfirmation').modal();
    //        $('#completeNoBtn').on('click', function () {
    //            $.post("/home/CreateInvoice", { customerId: customerId, orderId: $('#completeNoBtn').attr('data-orderId') }, function () {
    //                        $('#invoiceAlertMessage').html("Invoice generated successfuly!");
    //                        $('#invoiceAlertModal').modal();
    //                        //$('#orderDetailsModal').modal('toggle');
    //                        var s = $('#searchHistoryInput').val().toString();
    //                        var date = $('#filterDate').val();
    //                        var opt = $('#filterOpt option:selected').val();
    //                        var all = $('input[name=allDates]').is(':checked');
    //                        //$('.history').remove();
    //                        //IsLoaderForInput = true;
    //                        //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //                        //    populateOrders(ordersHistory);
    //                        //    $('#invoiceAlertModal').modal();
    //                        //})
    //                        t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
    //                        return false;
    //                    })
    //        })
    //        $('#completeCancelBtn').on('click', function () {
    //            return false;
    //        })
    //        $('#completeYesBtn').on('click', function () {
    //            t.attr('disabled', true);
    //            t.html("Completed");
    //            $.post("/home/GetOrderById", { orderId: $('#completeYesBtn').attr('data-orderId') }, function (order) {
    //                if (order.Status == "Delivery") {
    //                    status = "Delivered";
    //                }
    //                else {
    //                    status = "Picked up";
    //                }
    //                t.closest('tr').find('.status').html(status);
    //                $.post("/home/UpdateStatus", { orderId: $('#completeYesBtn').attr('data-orderId'), status: status }, function () {
    //                    //location.reload();
    //                })
    //            })
    //            if ($('#emailInput').val() != "") {
    //                var email = $('#emailInput').val();
    //                $('#emailInput').val("");
    //                if ($('#saveEmail').is(':checked')) {
    //                    $('#saveEmail').prop('checked', false);
    //                    $.post("/home/SaveEmail", { customerId: customerId, email: email }, function () {
    //                    })
    //                }
    //                $.post("/home/CreateInvoiceOtherEmail", { customerId: customerId, orderId: $('#completeYesBtn').attr('data-orderId'), email: email }, function () {
    //                    $('#invoiceAlertMessage').html('An invoice was sent to the email address you provided!');
    //                    //$('#orderDetailsModal').modal('toggle');
    //                    $('#invoiceAlertModal').modal();
    //                    var s = $('#searchHistoryInput').val().toString();
    //                    var date = $('#filterDate').val();
    //                    var opt = $('#filterOpt option:selected').val();
    //                    var all = $('input[name=allDates]').is(':checked');
    //                    //$('.history').remove();
    //                    //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //                    //    IsLoaderForInput = true;
    //                    //    populateOrders(ordersHistory);
    //                    //    $('#invoiceAlertModal').modal();
    //                    //})
    //                    t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
    //                })
    //            }
    //            else {
    //                if (customer.Email != "" && customer.Email != null) {
    //                    $.post("/home/CreateInvoiceEmail", { customerId: customerId, orderId: $('#completeYesBtn').attr('data-orderId') }, function () {
    //                        $('#invoiceAlertMessage').html("An invoice was sent via email to "+customer.FirstName+" "+customer.LastName+"!");
    //                        //$('#orderDetailsModal').modal('toggle');
    //                        $('#invoiceAlertModal').modal();
    //                        var s = $('#searchHistoryInput').val().toString();
    //                        var date = $('#filterDate').val();
    //                        var opt = $('#filterOpt option:selected').val();
    //                        var all = $('input[name=allDates]').is(':checked');
    //                        //$('.history').remove();
    //                        //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //                        //    IsLoaderForInput = true;
    //                        //    populateOrders(ordersHistory);
    //                        //    $('#invoiceAlertModal').modal();
    //                        //})
    //                        t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
    //                    })
    //                }
    //                else if (customer.Email == null && customer.Caterer == true) {
    //                    $.post("/home/CreateInvoice", { customerId: customerId, orderId: $('#completeYesBtn').attr('data-orderId') }, function () {
    //                        $('#invoiceAlertMessage').html("Invoice generated successfuly!");
    //                        $('#invoiceAlertModal').modal();
    //                        //$('#orderDetailsModal').modal('toggle');
    //                        var s = $('#searchHistoryInput').val().toString();
    //                        var date = $('#filterDate').val();
    //                        var opt = $('#filterOpt option:selected').val();
    //                        var all = $('input[name=allDates]').is(':checked');
    //                        //$('.history').remove();
    //                        //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //                        //    populateOrders(ordersHistory);
    //                        //})
    //                        t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
    //                    })
    //                }
    //                //else {
    //                //    $.post("/home/CreateInvoice", { customerId: customerId, orderId: orderId }, function () {
    //                //        $('#invoiceAlertMessage').html("Invoice generated successfuly!");
    //                //        $('#invoiceAlertModal').modal();
    //                //        //$('#orderDetailsModal').modal('toggle');
    //                //        var s = $('#searchHistoryInput').val().toString();
    //                //        var date = $('#filterDate').val();
    //                //        var opt = $('#filterOpt option:selected').val();
    //                //        var all = $('input[name=allDates]').is(':checked');
    //                //        //$('.history').remove();
    //                //        //IsLoaderForInput = true;
    //                //        //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //                //        //    populateOrders(ordersHistory);
    //                //        //    $('#invoiceAlertModal').modal();
    //                //        //})
    //                //        t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
    //                //    })
    //                //}
    //            }
    //        })
    //    })
    //})

    //$.post("/home/GetOrderById", { orderId: ordersId }, function (order) {
    //    if (order.Status == "Delivery") {
    //        status = "Delivered";
    //    }
    //    else {
    //        status = "Picked up";
    //    }
    //    t.closest('tr').find('.status').html(status);
    //    $.post("/home/UpdateStatus", { orderId: ordersId, status: status }, function () {
    //        //location.reload();
    //    })
    //})

    //$.post("/home/UpdateStatus", { orderId: ordersId, status: status }, function () {
    //    //location.reload();
    //})

    //    $.post("/home/GetCustomerById", { id: customerId }, function (customer) {
    //        if (customer.Email != "" && customer.Email != null) {
    //            $.post("/home/CreateInvoiceEmail", { customerId: customerId, orderId: ordersId }, function () {
    //                $('#invoiceAlertMessage').html('An invoice was sent via email to the customer!');
    //                //$('#orderDetailsModal').modal('toggle');
    //                $('#invoiceAlertModal').modal();
    //                var s = $('#searchHistoryInput').val().toString();
    //                var date = $('#filterDate').val();
    //                var opt = $('#filterOpt option:selected').val();
    //                var all = $('input[name=allDates]').is(':checked');
    //                //$('.history').remove();
    //                //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //                //    IsLoaderForInput = true;
    //                //    populateOrders(ordersHistory);
    //                //    $('#invoiceAlertModal').modal();
    //                //})
    //                t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
    //            })
    //        }
    //        else if (customer.Email == null && customer.Caterer == true) {
    //            $.post("/home/CreateInvoice", { customerId: customerId, orderId: ordersId }, function () {
    //                $('#invoiceAlertMessage').html("Invoice generated successfuly!\n\n<span style=" + '"' + "color:red" + '"' + ">This customer does not have an email address on file...</span>");
    //                $('#invoiceAlertModal').modal();
    //                //$('#orderDetailsModal').modal('toggle');
    //                var s = $('#searchHistoryInput').val().toString();
    //                var date = $('#filterDate').val();
    //                var opt = $('#filterOpt option:selected').val();
    //                var all = $('input[name=allDates]').is(':checked');
    //                //$('.history').remove();
    //                //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //                //    populateOrders(ordersHistory);
    //                //})
    //                t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
    //            })
    //        }

    //        else {
    //            $.post("/home/CreateInvoice", { customerId: customerId, orderId: ordersId }, function () {
    //                $('#invoiceAlertMessage').html("Invoice generated successfuly!");
    //                $('#invoiceAlertModal').modal();
    //                //$('#orderDetailsModal').modal('toggle');
    //                var s = $('#searchHistoryInput').val().toString();
    //                var date = $('#filterDate').val();
    //                var opt = $('#filterOpt option:selected').val();
    //                var all = $('input[name=allDates]').is(':checked');
    //                //$('.history').remove();
    //                //IsLoaderForInput = true;
    //                //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //                //    populateOrders(ordersHistory);
    //                //    $('#invoiceAlertModal').modal();
    //                //})
    //                t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
    //            })
    //        }
    //    })
    //    //$('.status').prop('checked', false);
    //    //$('#emailConfirm').prop('checked', true);
    //})

    //$('#historyTable').on('click', '.updateStatusBtn', function () {
    //    var t = $(this);
    //    t.attr('disabled', true);
    //    var ordersId = t.data('orderid');
    //    var customerId = t.data('customerid');
    //    var status = "";
    //    if (status == undefined) {
    //        status = "";
    //    }
    //    //$(t).data('orderid', "");
    //    //$(t).data('customerid', "");

    //    $.post("/home/GetOrderById", { orderId: ordersId }, function (order) {
    //        if (order.Status == "Delivery") {
    //            status = "Delivered";
    //        }
    //        else {
    //            status = "Picked up";
    //        }
    //        t.closest('tr').find('.status').html(status);
    //        $.post("/home/UpdateStatus", { orderId: ordersId, status: status }, function () {
    //            //location.reload();
    //        })
    //    })

    //    //$.post("/home/UpdateStatus", { orderId: ordersId, status: status }, function () {
    //    //    //location.reload();
    //    //})

    //    $.post("/home/GetCustomerById", { id: customerId }, function (customer) {
    //        if (customer.Email != "" && customer.Email != null) {
    //            $.post("/home/CreateInvoiceEmail", { customerId: customerId, orderId: ordersId }, function () {
    //                $('#invoiceAlertMessage').html('An invoice was sent via email to the customer!');
    //                //$('#orderDetailsModal').modal('toggle');
    //                $('#invoiceAlertModal').modal();
    //                var s = $('#searchHistoryInput').val().toString();
    //                var date = $('#filterDate').val();
    //                var opt = $('#filterOpt option:selected').val();
    //                var all = $('input[name=allDates]').is(':checked');
    //                //$('.history').remove();
    //                //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //                //    IsLoaderForInput = true;
    //                //    populateOrders(ordersHistory);
    //                //    $('#invoiceAlertModal').modal();
    //                //})
    //                t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
    //            })
    //        }
    //        else if (customer.Email == null && customer.Caterer == true) {
    //            $.post("/home/CreateInvoice", { customerId: customerId, orderId: ordersId }, function () {
    //                $('#invoiceAlertMessage').html("Invoice generated successfuly!\n\n<span style=" + '"' + "color:red" + '"' + ">This customer does not have an email address on file...</span>");
    //                $('#invoiceAlertModal').modal();
    //                //$('#orderDetailsModal').modal('toggle');
    //                var s = $('#searchHistoryInput').val().toString();
    //                var date = $('#filterDate').val();
    //                var opt = $('#filterOpt option:selected').val();
    //                var all = $('input[name=allDates]').is(':checked');
    //                //$('.history').remove();
    //                //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //                //    populateOrders(ordersHistory);
    //                //})
    //                t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
    //            })
    //        }

    //        else {
    //            $.post("/home/CreateInvoice", { customerId: customerId, orderId: ordersId }, function () {
    //                $('#invoiceAlertMessage').html("Invoice generated successfuly!");
    //                $('#invoiceAlertModal').modal();
    //                //$('#orderDetailsModal').modal('toggle');
    //                var s = $('#searchHistoryInput').val().toString();
    //                var date = $('#filterDate').val();
    //                var opt = $('#filterOpt option:selected').val();
    //                var all = $('input[name=allDates]').is(':checked');
    //                //$('.history').remove();
    //                //IsLoaderForInput = true;
    //                //$.post("/home/HistorySearch", { search: s, opt: opt, date: date, all: all }, function (ordersHistory) {
    //                //    populateOrders(ordersHistory);
    //                //    $('#invoiceAlertModal').modal();
    //                //})
    //                t.closest('tr').find('.viewInvoiceBtn').attr("disabled", false);
    //            })
    //        }
    //    })
    //    //$('.status').prop('checked', false);
    //    //$('#emailConfirm').prop('checked', true);
    //})

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


    $("#datepicker").datepicker();

    $('#requiredDate').datepicker();

    $('#filterDate').datepicker();

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

    $('#deductFromAcountBtn').on('click', function () {
        var customerId = $('#processPaymentBtn').data('customerid');
        var orderId = $('#processPaymentBtn').data('orderid');
        var amount = $('#amountPay').val();

        $.post("/home/DeductFromAccount", { customerId: customerId, orderId: orderId, amount: amount }, function () {
        })
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
        var phone1 = $('#searchInput').val();
        $('#phone1').val(phone1);
    })

    $('#customerSearchInput').on('input', function () {
        $('#customersDiv #custAlert').remove()
        $('#customersTable').find("tr:gt(0)").remove();
        var search = $(this).val();
        $.post("/home/SearchCustomer", { search: search }, function (c) {
            if (c.length === 0) {
                $('#customersDiv').append("<h3 id=custAlert>No matches found...</h3>")
            }
            for (i = 0; i < c.length; i++) {
                var account = 0;
                var balance = 0;
                if (c[i].Account != null) {
                    account = c[i].Account;
                    balance = c[i].Account;
                }
                var accParsed = (parseFloat(account, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString());
                if (account < 0) {
                    account = "$" + accParsed;
                }
                else {
                    account = "$" + accParsed;
                }
                var customerName = c[i].FirstName + " " + c[i].LastName;
                $('#customersTable').append("<tr data-customerid=" + c[i].Id + " data-balance=" + balance + " data-customer=" + '"' + customerName + '"' + "><td>" + c[i].LastName + " " + c[i].FirstName + "</td><td>" + c[i].Address + " " + c[i].City + " " + c[i].State + " " + c[i].Zip + "</td><td>" + c[i].Phone1 + " " + c[i].Phone2 + "</td><td>" + c[i].Cell1 + " " + c[i].Cell2 + "</td><td>" + c[i].Email + "</td><td class=" + '"' + "account" + '"' + ">" + account + "</td></tr>")
            }
        })
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

    $(document).on('click', '#customersTable tr', function () {
        var rowIndex = $(this).index();
        var customerId = $(this).data('customerid');
        var balance = $(this).data('balance');
        var customer = $(this).data('customer');
        $('#accountAjustments').attr('customerid', customerId);
        $('#accountAjustments').attr('balance', balance);
        $('#accountAjustments').attr('row', rowIndex);
        $('#customerEdit').attr('customerid', customerId);
        $('#customerEdit').attr('customer', customer);
        $('#customerEdit').attr('row', rowIndex);
        $('#customerEdit').attr('balance', balance);
        $('#accountHistory').attr('customerid', customerId)
        $('.modal-title').html(customer)
        $('#customerActionModal').modal();
    })

    $('#accountHistory').on('click', function () {
        $('#accountHistoryTable').find("tr:gt(0)").remove();
        var customerId = $(this).attr('customerid');
        $.post("/home/GetAccountTrans", { customerId: customerId }, function (h) {
            for (i = 0; i < h.length; i++) {
                $('#accountHistoryTable').append("<tr><td>" + ConvertJsonDate(h[i].Date) + "</td><td>" + (parseFloat(h[i].Amount, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString()) + "</td><td>" + h[i].Note + "</td></tr>")
            }
        })
        $('#accountHistoryModal').modal();
    })

    $('#accountAjustments').on('click', function () {
        var customerId = $(this).attr('customerid');
        var balance = $(this).attr('balance');
        var rowIndex = $(this).attr('row');
        $('#accountPaymentBtn').attr('customerid', customerId);
        $('#accountPaymentBtn').attr('balance', balance);
        $('#accountPaymentBtn').attr('row', rowIndex);
        $('#accountPaymentModal').modal();
    })

    $('#accountPaymentBtn').on('click', function () {
        var rowIndex = $(this).attr('row');
        var customerId = $(this).attr('customerid');
        var balance = parseFloat($(this).attr('balance'));
        var amount = parseFloat($('#amountPay').val());
        var note = $('#paymentNote').val();
        var balance = balance += amount;
        $('#accountAjustments').attr('balance', balance);
        $('#customerEdit').attr('balance', balance);
        $('#customersTable tr:eq(' + rowIndex + ')').data('balance', balance);

        $.post("/home/MakeAccountTrans", { customerId: customerId, amount: amount, note: note }, function () {
            $('#accountAlert').html("Account ajusted successfuly...");
            $('#customersTable tr:eq(' + rowIndex + ') .account').html((parseFloat(balance, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString()));
            $('#accountAlertModal').modal();
        })
    })

    $('#accountFullAmountCheckbox').change(function () {
        if (this.checked) {
            var balance = $('#accountPaymentBtn').attr('balance');
            //$('#amountPay').val(parseFloat(balance, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString())
            $('#amountPay').val(balance);
        }
        else {
            $('#amountPay').val("");
        }
    })

    $('#accountPaymentModal').on('hidden.bs.modal', function () {
        $('#amountPay').val("");
    })

    $('.categoryOpt').on('change', function () {

    })

    $('#customerEdit').on('click', function () {
        var customer = $(this).attr('customer');
        var rowIndex = $(this).attr('row');
        var id = $(this).attr('customerid');
        var balance = $(this).attr('balance');
        $('#EditCustomerSubmit').attr('customerid', id);
        $('#EditCustomerSubmit').attr('row', rowIndex);
        $('#EditCustomerSubmit').attr('balance', balance);
        $.post("/home/GetCustomerById", { id: id }, function (c) {
            if (c.Caterer === true) {
                $('#caterer').prop('checked', true);
            }
            else {
                $('#caterer').prop('checked', false);
            }
            $('#firstName').val(c.FirstName);
            $('#lastName').val(c.LastName);
            $('#address').val(c.Address);
            $('#city').val(c.City);
            $('#state').val(c.State);
            $('#zip').val(c.Zip);
            $('#phone1').val(c.Phone1);
            $('#cell1').val(c.Cell1);
            $('#phone2').val(c.Phone2);
            $('#cell2').val(c.Cell2);
            $('#email').val(c.Email);
            $('#EditCustomerModal').modal();
        })
    })

    $('#EditCustomerSubmit').on('click', function () {
        var rowIndex = $(this).attr('row');
        var id = $('#EditCustomerSubmit').attr('customerid', id);
        var caterer = false;
        var balance = $(this).attr('balance');
        if ($('#caterer').is(':checked')) {
            caterer = true;
        }
        $.post("/home/EditCustomer", { customerId: $(this).attr('customerid'), firstName: $('#firstName').val(), lastName: $('#lastName').val(), address: $('#address').val(), city: $('#city').val(), state: $('#state').val(), zip: $('#zip').val(), phone1: $('#phone1').val(), phone2: $('#phone2').val(), cell1: $('#cell1').val(), cell2: $('#cell2').val(), caterer: caterer, email: $('#email').val() }, function () {
            $('#accountAlert').html("Customer changes applied...");
            $('#accountAlertModal').modal();
            $('#customersTable tr:eq(' + rowIndex + ')').html("<td>" + $('#lastName').val() + " " + $('#firstName').val() + "</td><td>" + $('#address').val() + " " + $('#city').val() + " " + $('#state').val() + " " + $('#zip').val() + "</td><td>" + $('#phone1').val() + " " + $('#phone2').val() + "</td><td>" + $('#cell1').val() + " " + $('#cell2').val() + "</td><td>" + $('#email').val() + "</td><td class=" + '"' + "account" + '"' + ">" + (parseFloat(balance, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString()) + "</td>")
        })
    })

    $('.e').on('click', function () {
        location.href = "/home/order";
    })

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
            var catererDiscount = $(this).data('catererdiscount');
            var discount = 0;
            var category = $(this).data('categoryid');
            var quantity = $(this).find('input.q').val();
            itemCount += (parseInt(quantity));
            var price = $(this).data('price');
            if (quantity === undefined) {
                quantity === 0;
            }
            if (caterer === "true") {
                var t = (parseFloat(quantity) * parseFloat(price));
                if (catererDiscount < 1) {
                    discount = t * catererDiscount;
                    total += (t - discount);
                }
                else {
                    total += (t - catererDiscount * quantity);
                }
                //if (category === 1) {
                //    catererDiscount = 5;
                //    total += (t - catererDiscount * quantity);
                //}
                //else if (category === 2 || category === 3) {
                //    catererDiscount = t * 0.1;
                //    total += (t - catererDiscount);
                //}
                //else if (category === 4) {
                //    catererDiscount = 2.5;
                //    total += (t - catererDiscount * quantity);
                //}
                //else {
                //    total += t;
                //}
                if ((caterer === "true" || caterer === "True") && (catererDiscount < 1)) {
                    $(this).find('.price').html(t + "<span style=" + '"color: red"' + "> (-" + (parseFloat(discount, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString()) + ")</span>");
                }
                else if ((caterer === "true" || caterer === "True") && (catererDiscount >= 1)) {
                    $(this).find('.price').html(t + "<span style=" + '"color: red"' + "> (-" + (parseFloat((catererDiscount * quantity), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString()) + ")</span>");
                }
            }
            else {
                var t = (parseFloat(quantity) * parseFloat(price));
                total += t;
                $(this).find('.price').text(t)
            }

            $('#totalItems').text("Total items: " + itemCount);
            //if (total === NaN) {
            //    $('#total').text("Total: $" +(parseFloat(0, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString()));
            //}
            //else {
            //    if (getDiscount() < 1) {
            //        var d = total * getDiscount();
            //        total = total - d;
            //        $('#total').text("Total: $" + (parseFloat(total, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString()));
            //    }
            //    else {
            //        total = total - getDiscount();
            //        $('#total').text("Total: $" + (parseFloat(total, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString()));
            //    }
            //}
        });
        if (total === NaN) {
            $('#total').text("Total: $" + (parseFloat(0, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString()));
            $('#total').data('total', 0);
        }
        else {
            if (getDiscount() < 1) {
                var d = total * getDiscount();
                $('#total').text("Total: $" + (parseFloat(total - d, 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString()));
                $('#total').data('total', total - d);
            }
            else {
                $('#total').text("Total: $" + ((parseFloat((total - getDiscount()), 10).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, "$1,").toString())));
                $('#total').data('total', total - getDiscount());
            }
        }
    }
})



