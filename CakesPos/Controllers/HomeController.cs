using CakesPos.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CakesPos.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=CakesPos;Integrated Security=True";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Order()
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            OrdersViewModel ovm = new OrdersViewModel();
            ovm.categories = cpr.GetAllCategories();
            ovm.products = cpr.GetProductsByCategory(1);
            return View(ovm);
        }

        public ActionResult OrderHistory()
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders();
            //IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.requiredDate == DateTime.Today);
            return View(orders);
        }

        [HttpPost]
        public ActionResult OrderHistoryFilter(int x)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            DateTime today = DateTime.Today;
            IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.requiredDate >= today.AddDays(-x) && o.requiredDate < today.AddDays(1));
            return Json(orders, JsonRequestBehavior.AllowGet);
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
        public ActionResult AddCustomer(string firstName, string lastName, string address, string city, string state, string zip, string phone, string cell, bool caterer)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddCustomer(firstName, lastName, address, city, state, zip, phone, cell, caterer);
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public ActionResult Search(string search)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<Customer> customers = cpr.SearchCustomers(search);
            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult HistorySearch(string search)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<OrderHistoryViewModel> ordersHistory = cpr.SearchOrders(search);
            return Json(ordersHistory, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddOrder(int customerId, DateTime requiredDate, string deliveryOpt, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone, string creditCard, string expiration, string securityCode, string paymentMethod, decimal discount, string notes)
        {
            DateTime dateTime = DateTime.Now;
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            int orderId = cpr.AddOrder(customerId, dateTime, requiredDate, deliveryOpt, deliveryFirstName, deliveryLastName, deliveryAddress, deliveryCity, deliveryState, deliveryZip, phone, creditCard, expiration, securityCode, paymentMethod, discount, notes);
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
        public ActionResult GetTotalByOrderId(int id)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            decimal total = cpr.GetTotalByOrderId(id);
            return Json(total, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateInventory(int id, int amount)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.UpdateInventory(id, amount);
            return null;
        }

    }
}
