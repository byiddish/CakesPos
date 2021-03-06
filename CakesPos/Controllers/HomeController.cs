﻿using CakesPos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Authentications.Data;
using System.IO;
using System.Net;

namespace CakesPos.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=CakesPos;Integrated Security=True";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrderList()
        {
            List<OrderDetailsViewModel> od = new List<OrderDetailsViewModel>();
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.requiredDate == DateTime.Today && o.invoice == false);
            foreach (OrderHistoryViewModel o in orders)
            {
                od.Add(cpr.GetOrderDetails(o.customerId, o.id));
            }
            return View(od);
        }

        //[Authorize]
        public ActionResult Order()
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            OrdersViewModel ovm = new OrdersViewModel();
            ovm.categories = cpr.GetAllCategories();
            ovm.products = cpr.GetProductsByCategory(1);
            ovm.productAvailability = cpr.GetProductAvailability(DateTime.Today.Date.AddMonths(-1), DateTime.Today.Date.AddDays(7), 1);
            return View(ovm);
        }

        //[HttpPost]
        //public ActionResult Order(int customerId, int orderId)
        //{
        //    CakesPosRepository cpr = new CakesPosRepository(_connectionString);
        //    OrdersViewModel ovm = new OrdersViewModel();
        //    ovm.categories = cpr.GetAllCategories();
        //    ovm.products = cpr.GetProductsByCategory(1);
        //    ovm.order = cpr.GetOrderById(orderId);
        //    ovm.orderDetails = cpr.GetOrderDetailsById(orderId);
        //    return View(ovm);
        //}

        //[HttpPost]
        public ActionResult EditOrder(int customerId, int orderId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            EditOrdersViewModel ovm = new EditOrdersViewModel();
            ovm.categories = cpr.GetAllCategories();
            ovm.products = cpr.GetProductsByCategory(1);
            //ovm.order = cpr.GetOrderById(orderId);
            //ovm.orderDetails = cpr.GetOrderDetailsById();
            //ovm.customer = cpr.GetCustomerById(customerId);
            //ovm.orderedProducts=cpr.
            ovm.productAvailability = cpr.GetProductAvailability(DateTime.Today.AddMonths(-1), DateTime.Today.AddDays(3), 1);
            ovm.orderDetails = cpr.GetOrderDetails(customerId, orderId);
            return View(ovm);
        }

        [HttpPost]
        public ActionResult OpenOrder(int orderId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.openOrder(orderId);
            return null;
        }

        public ActionResult OrderHistory()
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            //IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders();
            IEnumerable<OrderHistoryViewModel> orders = cpr.SearchOrders("", "open", DateTime.Today.Date, false);
            return View(orders);
        }

        //[HttpPost]
        //public ActionResult GetOrderHistoryFiltered(string name, int date, string opt)
        //{

        //}

        public ActionResult Delivery()
        {
            List<OrderDetailsViewModel> od = new List<OrderDetailsViewModel>();
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == "Delivery" && o.requiredDate == DateTime.Today && o.invoice == false);
            foreach (OrderHistoryViewModel o in orders)
            {
                od.Add(cpr.GetOrderDetails(o.customerId, o.id));
            }
            return View(od);
        }

        //[HttpGet]
        public ActionResult GetCatagories()
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            return Json(cpr.GetAllCategories(), JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult DeliveryFilter(int x)
        //{
        //    List<OrderDetailsViewModel> od = new List<OrderDetailsViewModel>();
        //    CakesPosRepository cpr = new CakesPosRepository(_connectionString);
        //    DateTime today = DateTime.Today;
        //    if (x == -1)
        //    {
        //        IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == "Delivery" && o.requiredDate >= today.AddDays(x) && o.requiredDate < today && o.invoice == false);
        //        foreach (OrderHistoryViewModel o in orders)
        //        {
        //            od.Add(cpr.GetOrderDetails(o.customerId, o.id));
        //        }
        //        return Json(od, JsonRequestBehavior.AllowGet);
        //    }
        //    else if (x <= 0)
        //    {
        //        IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == "Delivery" && o.requiredDate >= today.AddDays(x) && o.requiredDate <= today && o.invoice == false);
        //        foreach (OrderHistoryViewModel o in orders)
        //        {
        //            od.Add(cpr.GetOrderDetails(o.customerId, o.id));
        //        }
        //        return Json(od, JsonRequestBehavior.AllowGet);
        //    }
        //    else if (x == 1)
        //    {
        //        IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == "Delivery" && o.requiredDate == today.AddDays(x) && o.invoice == false);
        //        foreach (OrderHistoryViewModel o in orders)
        //        {
        //            od.Add(cpr.GetOrderDetails(o.customerId, o.id));
        //        }
        //        return Json(od, JsonRequestBehavior.AllowGet);
        //    }
        //    else if (x == 8)
        //    {
        //        IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == "Delivery" && o.invoice == false);
        //        foreach (OrderHistoryViewModel o in orders)
        //        {
        //            od.Add(cpr.GetOrderDetails(o.customerId, o.id));
        //        }
        //        return Json(od, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == "Delivery" && o.requiredDate <= today.AddDays(x) && o.requiredDate >= today && o.invoice == false);
        //        foreach (OrderHistoryViewModel o in orders)
        //        {
        //            od.Add(cpr.GetOrderDetails(o.customerId, o.id));
        //        }
        //        return Json(od, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public ActionResult DeliveryFilter(int x, string deliveryOpt)
        {
            List<OrderDetailsViewModel> od = new List<OrderDetailsViewModel>();
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            DateTime today = DateTime.Today;
            if (x == -1)
            {
                IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == deliveryOpt && o.requiredDate >= today.AddDays(x) && o.requiredDate < today && o.invoice == false);
                foreach (OrderHistoryViewModel o in orders)
                {
                    od.Add(cpr.GetOrderDetails(o.customerId, o.id));
                }
                return Json(od, JsonRequestBehavior.AllowGet);
            }
            else if (x <= 0)
            {
                IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == deliveryOpt && o.requiredDate >= today.AddDays(x) && o.requiredDate <= today && o.invoice == false);
                foreach (OrderHistoryViewModel o in orders)
                {
                    od.Add(cpr.GetOrderDetails(o.customerId, o.id));
                }
                return Json(od, JsonRequestBehavior.AllowGet);
            }
            else if (x == 1)
            {
                IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == deliveryOpt && o.requiredDate == today.AddDays(x) && o.invoice == false);
                foreach (OrderHistoryViewModel o in orders)
                {
                    od.Add(cpr.GetOrderDetails(o.customerId, o.id));
                }
                return Json(od, JsonRequestBehavior.AllowGet);
            }
            else if (x == 8)
            {
                IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == deliveryOpt && o.invoice == false);
                foreach (OrderHistoryViewModel o in orders)
                {
                    od.Add(cpr.GetOrderDetails(o.customerId, o.id));
                }
                return Json(od, JsonRequestBehavior.AllowGet);
            }
            else
            {
                IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == deliveryOpt && o.requiredDate <= today.AddDays(x) && o.requiredDate >= today && o.invoice == false);
                foreach (OrderHistoryViewModel o in orders)
                {
                    od.Add(cpr.GetOrderDetails(o.customerId, o.id));
                }
                return Json(od, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public ActionResult GetProductsByCategoryWithAvailability(int categoryId)
        //{
        //    CakesPosRepository cpr = new CakesPosRepository(_connectionString);
        //    IEnumerable<InventoryViewModel> products = cpr.GetProductAvailability(DateTime.Today.Date, DateTime.Today.Date.AddDays(7), categoryId);
        //    return Json(products);
        //}

        [HttpPost]
        public ActionResult OrderHistoryFilter(int x)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            DateTime today = DateTime.Today;
            if (x == -1)
            {
                IEnumerable<OrderHistoryViewModel> yesterdaysOrders = cpr.GetOrders().Where(o => o.requiredDate >= today.AddDays(x) && o.requiredDate < today);
                return Json(yesterdaysOrders.ToList(), JsonRequestBehavior.AllowGet);
            }
            else if (x <= 0)
            {
                IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.requiredDate >= today.AddDays(x) && o.requiredDate <= today);
                return Json(orders.OrderByDescending(o => o.requiredDate).ToList(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.requiredDate <= today.AddDays(x) && o.requiredDate >= today);
                return Json(orders.OrderByDescending(o => o.requiredDate).ToList(), JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public ActionResult OrderHistoryWeek()
        //{
        //    CakesPosRepository cpr = new CakesPosRepository(_connectionString);
        //    IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.requiredDate >= DateTime.Today.AddDays(-7));
        //    return Json(orders, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult OrderHistoryLast30Days()
        //{
        //    CakesPosRepository cpr = new CakesPosRepository(_connectionString);
        //    IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.requiredDate >= DateTime.Today.AddDays(-30));
        //    return Json(orders, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult Admin()
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            return View(cpr.GetAllCategories());
        }

        public ActionResult Inventory()
        {
            DateTime min = DateTime.Now.Date.AddMonths(-1);
            DateTime max = DateTime.Now.Date.AddDays(3);
            InventoryByCategoryModel ibcm = new InventoryByCategoryModel();
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            ibcm.inventory = cpr.GetInventory(min, max);
            ibcm.categories = cpr.GetAllCategories();
            ViewBag.category = 1;
            ViewBag.minDate = min;
            ViewBag.maxDate = max;
            ViewBag.dayReq = 4;
            return View(ibcm);
        }

        //[HttpPost]
        //public ActionResult AddCustomProduct(string productName, int restockAmount, decimal price, decimal catererDiscount, int inStock, int sortIndex)
        //{
        //    CakesPosRepository cpr = new CakesPosRepository(_connectionString);
        //    cpr.AddProduct(productName, price, catererDiscount, restockAmount, inStock, "default.jpg", 5);
        //    return null;
        //}

        [HttpPost]
        public ActionResult GetOrderById(int orderId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            return Json(cpr.GetOrderById(orderId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Inventory(DateTime min, DateTime max, int dayReq, int categoryId)
        {
            InventoryByCategoryModel ibcm = new InventoryByCategoryModel();
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            ibcm.inventory = cpr.GetInventory(min, max).Where(c => c.product.CategoryId == categoryId);
            ibcm.categories = cpr.GetAllCategories();
            ViewBag.category = categoryId;
            ViewBag.minDate = min;
            ViewBag.maxDate = max;
            ViewBag.dayReq = dayReq;
            return View(ibcm);
        }

        public ActionResult AddCategory(string categoryName)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddCategory(categoryName);
            return RedirectToAction("Admin");


        }

        [HttpPost]
        public ActionResult AddUser(string username, string name, string password)
        {
            var mgr = new UserManager(Properties.Settings.Default.ContstrAuth);
            mgr.AddUser(username, password, name);

            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var mgr = new UserManager(Properties.Settings.Default.ContstrAuth);
            var user = mgr.GetUser(username, password);
            if (user == null)
            {
                return View(new UserViewModel { Name = username });
            }

            FormsAuthentication.SetAuthCookie(user.UserName, true);
            return RedirectToAction("Order");
        }

        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult AddNewProduct(string productName, decimal price, decimal catererDiscount, int restockAmount, int inStock, HttpPostedFileBase image, int categoryId, int sortIndex)
        {
            if (image == null)
            {
                CakesPosRepository cpr = new CakesPosRepository(_connectionString);
                cpr.AddProduct(productName, price, catererDiscount, restockAmount, inStock, "default.jpg", categoryId, sortIndex);
                return RedirectToAction("Admin");
            }
            else
            {
                Guid g = Guid.NewGuid();

                image.SaveAs(Server.MapPath("~/Uploads/" + g + ".jpg"));

                CakesPosRepository cpr = new CakesPosRepository(_connectionString);
                cpr.AddProduct(productName, price, catererDiscount, restockAmount, inStock, g + ".jpg", categoryId, sortIndex);
                return RedirectToAction("Admin");
            }
        }

        [HttpPost]
        public ActionResult GetProduct(int productId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            return Json((cpr.GetProductById(productId)), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditProduct(int? productId, int? categoryId, string productName, decimal? price, decimal? catererDiscount, int? restockAmount, HttpPostedFileBase image, bool? discontinued)
        {
            Product p = new Product();
            p.ProductName = productName;
            p.Price = (decimal)price;
            p.CatererDiscount = (decimal)catererDiscount;
            p.RestockAmount = restockAmount;
            p.CategoryId = (int)categoryId;
            if (image != null)
            {
                Guid g = Guid.NewGuid();
                image.SaveAs(Server.MapPath("~/Uploads/" + g + ".jpg"));
                p.Image = g + ".jpg";
            }
            else
            {
                p.Image = "";
            }
            p.Discontinued = discontinued;
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.EditProduct(p, (int)productId);
            return RedirectToAction("Inventory");
        }

        [HttpPost]
        public ActionResult GetProductsByCategory(int categoryId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<Product> products = cpr.GetProductsByCategory(categoryId);
            return Json(products, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetProductAvailabilityByProductId(int productId)
        {
            DateTime min = DateTime.Today.Date.AddMonths(-1);
            DateTime max = DateTime.Today.Date.AddDays(7);
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            return Json(cpr.GetProductAvailabilityByProductId(min, max, productId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddNewCustomer(string firstName, string lastName, string address, string city, string state, string zip, string phone1, string phone2, string cell1, string cell2, bool caterer, string email)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            return Json(cpr.AddCustomer(firstName, lastName, address, city, state, zip, phone1, phone2, cell1, cell2, caterer, email), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(string search)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<Customer> customers = cpr.SearchCustomers(search);
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult HistorySearch(string search, string opt, DateTime date, bool all)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<OrderHistoryViewModel> ordersHistory = cpr.SearchOrders(search, opt, date, all);
            return Json(ordersHistory, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddOrder(int customerId, DateTime requiredDate, string deliveryOpt, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone1, string phone2, string cell1, string cell2, string creditCard, string expiration, string securityCode, string paymentMethod, decimal discount, string notes, string greetings, string deliveryNote, bool paid, decimal deliveryCharge)
        {
            DateTime dateTime = DateTime.Now;
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            int orderId = cpr.AddOrder(customerId, dateTime, requiredDate, deliveryOpt, deliveryFirstName, deliveryLastName, deliveryAddress, deliveryCity, deliveryState, deliveryZip, phone1, phone2, cell1, cell2, creditCard, expiration, securityCode, paymentMethod, discount, notes, greetings, deliveryNote, paid, deliveryCharge);
            return Json(orderId);
        }

        [HttpPost]
        public ActionResult AddOrderDetails(int orderId, int productId, decimal unitPrice, int quantity)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddOrderDetails(orderId, productId, unitPrice, quantity);
            return null;
        }

        [HttpPost]
        public ActionResult GetOrderHistory(int orderId, int customerId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            OrderDetailsViewModel orderHistory = cpr.GetOrderDetails(orderId, customerId);
            return Json(orderHistory, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetOrderStatus(int orderId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            return Json(cpr.GetLatestStatusById(orderId), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTotalByOrderId(int id, int customerId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            decimal total = cpr.GetTotalByOrderId(id, customerId);
            return Json(total, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateInventory(int id, int amount)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.UpdateInventory(id, amount);
            return null;
        }

        [HttpPost]
        public ActionResult GetCustomerById(int id)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            Customer customer = cpr.GetCustomerById(id);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditCustomer(int customerId, string firstName, string lastName, string address, string city, string state, string zip, string phone1, string phone2, string cell1, string cell2, bool caterer, string email)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.EditCustomer(customerId, firstName, lastName, address, city, state, zip, phone1, phone2, cell1, cell2, caterer, email);
            return null;
        }

        [HttpPost]
        public ActionResult MakePayment(int customerId, int orderId, decimal amount, string note, string paymentMethod)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.MakePayment(customerId, orderId, amount, note, paymentMethod);
            return null;
        }

        [HttpPost]
        public ActionResult UpdateInvoicesPaid(int statementId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.UpdateInvoicesPaid(statementId);
            return null;
        }

        [HttpPost]
        public ActionResult UpdateOrderById(int orderId, int customerId, DateTime requiredDate, string deliveryOpt, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone1, string phone2, string cell1, string cell2, string creditCard, string expiration, string securityCode, string paymentMethod, decimal discount, string notes, string greetings, string deliveryNote, bool paid, decimal deliveryCharge)
        {
            DateTime dateTime = DateTime.Now;
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.DeleteOrderDetailsById(orderId);
            cpr.UpdateOrderById(orderId, customerId, dateTime, requiredDate, deliveryOpt, deliveryFirstName, deliveryLastName, deliveryAddress, deliveryCity, deliveryState, deliveryZip, phone1, phone2, cell1, cell2, creditCard, expiration, securityCode, paymentMethod, discount, notes, greetings, deliveryNote, paid, deliveryCharge);
            return null;
        }

        [HttpPost]
        public ActionResult DeletePayments(int orderId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.DeletePaymentsById(orderId);
            return null;
        }

        [HttpPost]
        public ActionResult DeleteOrder(int id)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.DeleteStatusById(id);
            cpr.DeletePaymentsById(id);
            cpr.DeleteOrderDetailsById(id);
            cpr.DeleteOrderById(id);
            return null;
        }

        [HttpPost]
        public ActionResult UpdateStatus(int orderId, string status)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddStatus(orderId, status);
            return null;
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmailInvoice(int customerId, int orderId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            InvoiceManager i = new InvoiceManager();
            Customer c = cpr.GetCustomerById(customerId);
            i.EmailInvoice(@"C:\inetpub\sites\CakesPos\InvoicesPdf\" + orderId + ".pdf", c.Email);
            return null;
        }

        [HttpPost]
        public ActionResult CreateInvoiceEmail(int customerId, int orderId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            OrderDetailsViewModel o = cpr.GetOrderDetails(customerId, orderId);
            InvoiceManager i = new InvoiceManager();
            i.CreateInvoicePDF(o);
            i.EmailInvoice(@"C:\inetpub\sites\CakesPos\InvoicesPdf\" + orderId + ".pdf", o.customer.Email);
            return null;
        }

        [HttpPost]
        public ActionResult CreateInvoiceOtherEmail(int customerId, int orderId, string email)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            OrderDetailsViewModel o = cpr.GetOrderDetails(customerId, orderId);
            InvoiceManager i = new InvoiceManager();
            i.CreateInvoicePDF(o);
            i.EmailInvoice(@"C:\inetpub\sites\CakesPos\InvoicesPdf\" + orderId + ".pdf", email);
            return null;
        }

        [HttpPost]
        public ActionResult CreateInvoice(int customerId, int orderId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            OrderDetailsViewModel o = cpr.GetOrderDetails(customerId, orderId);
            InvoiceManager i = new InvoiceManager();
            i.CreateInvoicePDF(o);
            return null;
        }

        [HttpPost]
        public ActionResult DeductFromAccount(int customerId, int orderId, decimal amount)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.MakeAccountTrans(customerId, -amount, "Order #" + orderId + " Withdrawal");
            return null;
        }

        [HttpPost]
        public ActionResult UnCompleteOrder(int orderId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.UnCompleteOrder(orderId);
            return null;
        }

        [HttpPost]
        public ActionResult StatementDeductFromAccount(int customerId, int statementId, decimal amount, string note)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.sDeductFromAccount(customerId, statementId, amount, note);
            return null;
        }

        public ActionResult Statements()
        {
            //DateTime min = DateTime.Now.AddMonths(-1).Date;
            //DateTime max = DateTime.Now.Date;
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<OrderHistoryViewModel> orders = cpr.GetCatererOrdersForStatements();
            orders = orders.OrderBy(o => o.lastName);
            return View(orders);
        }

        [HttpPost]
        public ActionResult GenerateStatementEmail(int customerId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            int id = cpr.GenerateStatement(customerId);
            StatementsModel s = cpr.GetStatementsForPdf(id, customerId);
            StatementManager sm = new StatementManager();
            sm.CreateStatementPDF(s, @"C:\inetpub\sites\CakesPos\StatementsPdf\" + s.Statement.Id + ".pdf");
            cpr.AddStatementFilePath(s.Statement.Id, @"C:\inetpub\sites\CakesPos\StatementsPdf\" + s.Statement.Id + ".pdf");
            sm.EmailStatement(@"C:\inetpub\sites\CakesPos\StatementsPdf\" + s.Statement.Id + ".pdf", s.Orders.FirstOrDefault().customer.Email, s.Statement.Date.ToShortDateString());
            return null;
        }

        [HttpPost]
        public ActionResult GenerateStatementPrint(int customerId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            int id = cpr.GenerateStatement(customerId);
            StatementsModel s = cpr.GetStatementsForPdf(id, customerId);
            StatementManager sm = new StatementManager();
            sm.CreateStatementPDF(s, @"C:\inetpub\sites\CakesPos\StatementsPdf\" + s.Statement.Id + ".pdf");
            cpr.AddStatementFilePath(s.Statement.Id, @"C:\inetpub\sites\CakesPos\StatementsPdf\" + s.Statement.Id + ".pdf");
            return Json(id, JsonRequestBehavior.AllowGet);
            //sm.EmailStatement(@"C:\Users\Barry\Documents\Pdf-Statements\" + s.Statement.Id + ".pdf", s.Orders.FirstOrDefault().customer.Email, s.Statement.Date.ToShortDateString());
            //Statement s=cpr.GetStatementByCustomerId(customerId);
            //StatementManager sm = new StatementManager();
            //sm.CreateStatementPDF(s, @"C:\Users\Barry\Documents\Pdf-Statements\" + s.StatementNumber + ".pdf");
            //sm.EmailStatement(@"C:\Users\Barry\Documents\Pdf-Statements\" + s.StatementNumber + ".pdf", s.Orders.FirstOrDefault().customer.Email, s.StatementDate.ToShortDateString());
            //cpr.AddStatementsFilePath()Path.Combine(AppDomain.CurrentDomain.BaseDirectory,s.Statement.Id + ".pdf");
        }

        public FileStreamResult GetPDF(int statementId)
        {
            FileStream fs = new FileStream(@"C:\inetpub\sites\CakesPos\StatementsPdf\" + statementId + ".pdf", FileMode.Open, FileAccess.Read);
            return File(fs, "application/pdf");
        }

        public FileStreamResult GetInvoice(int invoiceId)
        {
            FileStream fs = new FileStream(@"C:\inetpub\sites\CakesPos\InvoicesPdf\" + invoiceId + ".pdf", FileMode.Open, FileAccess.Read);
            return File(fs, "application/pdf");
        }

        public ActionResult StatementPdf()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GenerateStatementPrintEmail(int customerId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            int id = cpr.GenerateStatement(customerId);
            StatementsModel s = cpr.GetStatementsForPdf(id, customerId);
            StatementManager sm = new StatementManager();
            sm.CreateStatementPDF(s, @"C:\inetpub\sites\CakesPos\StatementsPdf\" + s.Statement.Id + ".pdf");
            cpr.AddStatementFilePath(s.Statement.Id, @"C:\inetpub\sites\CakesPos\StatementsPdf\" + s.Statement.Id + ".pdf");
            sm.EmailStatement(@"C:\inetpub\sites\CakesPos\StatementsPdf\" + s.Statement.Id + ".pdf", s.Orders.FirstOrDefault().customer.Email, s.Statement.Date.ToShortDateString());
            return Json(id, JsonRequestBehavior.AllowGet);
            //sm.EmailStatement(@"C:\Users\Barry\Documents\Pdf-Statements\" + s.Statement.Id + ".pdf", s.Orders.FirstOrDefault().customer.Email, s.Statement.Date.ToShortDateString());
            //Statement s=cpr.GetStatementByCustomerId(customerId);
            //StatementManager sm = new StatementManager();
            //sm.CreateStatementPDF(s, @"C:\Users\Barry\Documents\Pdf-Statements\" + s.StatementNumber + ".pdf");
            //sm.EmailStatement(@"C:\Users\Barry\Documents\Pdf-Statements\" + s.StatementNumber + ".pdf", s.Orders.FirstOrDefault().customer.Email, s.StatementDate.ToShortDateString());
            //cpr.AddStatementsFilePath()
        }

        [HttpGet]
        public ActionResult GetOpenStatements()
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<StatementsModel> statements = cpr.GetAllOpenStatements();
            return Json(statements, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveEmail(int customerId, string email)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.SaveEmail(customerId, email);
            return null;
        }

        [HttpPost]
        public ActionResult AddStatementPayment(int customerId, int statementId, decimal amount, string paymentNote)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddStatementPayment(customerId, statementId, amount, paymentNote);
            return null;
        }


        [HttpPost]
        public ActionResult Statements(DateTime min, DateTime max)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<OrderHistoryViewModel> orders = cpr.GetCatererOrdersByDate(min, max);
            orders = orders.OrderBy(o => o.lastName);
            return Json(orders, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetStatementsFiltered(string search, string filter)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            return Json(cpr.GetStatementsFiltered(search, filter), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Customers()
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            return View(cpr.GetAllCustomers());
        }

        [HttpPost]
        public ActionResult SearchCustomer(string search)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            return Json(cpr.SearchCustomers(search), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MakeAccountTrans(int customerId, decimal amount, string note)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.MakeAccountTrans(customerId, amount, note);
            return null;
        }

        [HttpPost]
        public ActionResult GetAccountTrans(int customerId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            return Json(cpr.GetAccountTrans(customerId), JsonRequestBehavior.AllowGet);
        }

    }
}
