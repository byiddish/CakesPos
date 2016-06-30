using CakesPos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Authentications.Data;

namespace CakesPos.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=CakesPos;Integrated Security=True";

        public ActionResult Index()
        {
            return View();
        }

        //[Authorize]
        public ActionResult Order()
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            OrdersViewModel ovm = new OrdersViewModel();
            ovm.categories = cpr.GetAllCategories();
            ovm.products = cpr.GetProductsByCategory(1);
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
            ovm.orderDetails = cpr.GetOrderDetails(customerId, orderId);
            return View(ovm);
        }

        public ActionResult OrderHistory()
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            //IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders();
            IEnumerable<OrderHistoryViewModel> orders = cpr.SearchOrders("", 0, "open");
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
            IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == "Delivery" && o.requiredDate == DateTime.Today);
            foreach (OrderHistoryViewModel o in orders)
            {
                od.Add(cpr.GetOrderDetails(o.customerId, o.id));
            }
            return View(od);
        }

        [HttpPost]
        public ActionResult DeliveryFilter(int x)
        {
            List<OrderDetailsViewModel> od = new List<OrderDetailsViewModel>();
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            DateTime today = DateTime.Today;
            if (x <= 0)
            {
                IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == "Delivery" && o.requiredDate >= today.AddDays(x) && o.requiredDate <= today);
                foreach (OrderHistoryViewModel o in orders)
                {
                    od.Add(cpr.GetOrderDetails(o.customerId, o.id));
                }
                return Json(od, JsonRequestBehavior.AllowGet);
            }
            else if (x == -1)
            {
                IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == "Delivery" && o.requiredDate >= today.AddDays(x) && o.requiredDate < today);
                foreach (OrderHistoryViewModel o in orders)
                {
                    od.Add(cpr.GetOrderDetails(o.customerId, o.id));
                }
                return Json(od, JsonRequestBehavior.AllowGet);
            }
            else
            {
                IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == "Delivery" && o.requiredDate <= today.AddDays(x) && o.requiredDate >= today);
                foreach (OrderHistoryViewModel o in orders)
                {
                    od.Add(cpr.GetOrderDetails(o.customerId, o.id));
                }
                return Json(od, JsonRequestBehavior.AllowGet);
            }
        }

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

        [AllowAnonymous]
        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult Inventory()
        {
            DateTime min = DateTime.Now;
            DateTime max = DateTime.Now.AddDays(7);
            InventoryByCategoryModel ibcm = new InventoryByCategoryModel();
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            ibcm.inventory = cpr.GetInventory(min, max);
            ibcm.categories = cpr.GetAllCategories();
            return View(ibcm);
        }

        [HttpPost]
        public ActionResult Inventory(DateTime min, DateTime max)
        {
            InventoryByCategoryModel ibcm = new InventoryByCategoryModel();
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            ibcm.inventory = cpr.GetInventory(min, max);
            ibcm.categories = cpr.GetAllCategories();
            return View(ibcm);
        }

        public ActionResult AddCategory(string categoryName)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddCategory(categoryName);
            return RedirectToAction("Admin");


        }

        [AllowAnonymous]
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

        public ActionResult AddNewProduct(string productName, decimal price, int inStock, HttpPostedFileBase image, int categoryId)
        {
            Guid g = Guid.NewGuid();

            image.SaveAs(Server.MapPath("~/Uploads/" + g + ".jpg"));

            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddProduct(productName, price, inStock, g + ".jpg", categoryId);
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public ActionResult GetProductsByCategory(int categoryId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<Product> products = cpr.GetProductsByCategory(categoryId);
            return Json(products);
        }

        [HttpPost]
        public ActionResult AddCustomer(string firstName, string lastName, string address, string city, string state, string zip, string phone, string cell, bool caterer, string email)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddCustomer(firstName, lastName, address, city, state, zip, phone, cell, caterer, email);
            return null;
        }

        [HttpPost]
        public ActionResult Search(string search)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<Customer> customers = cpr.SearchCustomers(search);
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult HistorySearch(string search, int x, string opt)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<OrderHistoryViewModel> ordersHistory = cpr.SearchOrders(search, x, opt);
            return Json(ordersHistory, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddOrder(int customerId, DateTime requiredDate, string deliveryOpt, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone, string creditCard, string expiration, string securityCode, string paymentMethod, decimal discount, string notes, string greetings, string deliveryNote, bool paid)
        {
            DateTime dateTime = DateTime.Now;
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            int orderId = cpr.AddOrder(customerId, dateTime, requiredDate, deliveryOpt, deliveryFirstName, deliveryLastName, deliveryAddress, deliveryCity, deliveryState, deliveryZip, phone, creditCard, expiration, securityCode, paymentMethod, discount, notes, greetings, deliveryNote, paid);
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
            double total = cpr.GetTotalByOrderId(id, customerId);
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
        public ActionResult EditCustomer(int customerId, string firstName, string lastName, string address, string city, string state, string zip, string phone, string cell, bool caterer, string email)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.EditCustomer(customerId, firstName, lastName, address, city, state, zip, phone, cell, caterer, email);
            return null;
        }

        [HttpPost]
        public ActionResult MakePayment(int customerId, int orderId, decimal amount, string note)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.MakePayment(customerId, orderId, amount, note);
            return null;
        }

        [HttpPost]
        public ActionResult UpdateOrderById(int orderId, int customerId, DateTime requiredDate, string deliveryOpt, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone, string creditCard, string expiration, string securityCode, string paymentMethod, decimal discount, string notes, string greetings, string deliveryNote, bool paid)
        {
            DateTime dateTime = DateTime.Now;
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.DeleteOrderDetailsById(orderId);
            cpr.UpdateOrderById(orderId, customerId, dateTime, requiredDate, deliveryOpt, deliveryFirstName, deliveryLastName, deliveryAddress, deliveryCity, deliveryState, deliveryZip, phone, creditCard, expiration, securityCode, paymentMethod, discount, notes, greetings, deliveryNote, paid);
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
        public ActionResult CreateInvoiceEmail(int customerId, int orderId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            OrderDetailsViewModel o = cpr.GetOrderDetails(customerId, orderId);
            InvoiceManager i = new InvoiceManager();
            i.CreateInvoicePDF(o);
            i.EmailInvoice(@"C:\Users\Barry\Documents\Pdf-Files\" + orderId + ".pdf", o.customer.Email);
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
            cpr.DeductFromAccount(customerId, orderId, amount);
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
            sm.CreateStatementPDF(s, @"C:\Users\Barry\documents\visual studio 2013\Projects\CakesPos\CakesPos\Statements-Pdf\" + s.Statement.Id + ".pdf");
            cpr.AddStatementFilePath(s.Statement.Id, @"C:\Users\Barry\documents\visual studio 2013\Projects\CakesPos\CakesPos\Statements-Pdf\" + s.Statement.Id + ".pdf");
            //sm.EmailStatement(@"C:\Users\Barry\Documents\Pdf-Statements\" + s.Statement.Id + ".pdf", s.Orders.FirstOrDefault().customer.Email, s.Statement.Date.ToShortDateString());
            //Statement s=cpr.GetStatementByCustomerId(customerId);
            //StatementManager sm = new StatementManager();
            //sm.CreateStatementPDF(s, @"C:\Users\Barry\Documents\Pdf-Statements\" + s.StatementNumber + ".pdf");
            //sm.EmailStatement(@"C:\Users\Barry\Documents\Pdf-Statements\" + s.StatementNumber + ".pdf", s.Orders.FirstOrDefault().customer.Email, s.StatementDate.ToShortDateString());
            //cpr.AddStatementsFilePath()
            return null;
        }

        [HttpPost]
        public ActionResult GenerateStatementPrint(int customerId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            int id = cpr.GenerateStatement(customerId);
            StatementsModel s = cpr.GetStatementsForPdf(id, customerId);
            StatementManager sm = new StatementManager();
            sm.CreateStatementPDF(s, @"C:\Users\Barry\documents\visual studio 2013\Projects\CakesPos\CakesPos\Statements-Pdf\" + s.Statement.Id + ".pdf");
            cpr.AddStatementFilePath(s.Statement.Id, @"C:\Users\Barry\documents\visual studio 2013\Projects\CakesPos\CakesPos\Statements-Pdf\" + s.Statement.Id + ".pdf");
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
