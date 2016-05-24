﻿using CakesPos.Data;
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
            IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.requiredDate == DateTime.Today);
            return View(orders);
        }

        public ActionResult Delivery()
        {
            List<OrderDetailsViewModel> od = new List<OrderDetailsViewModel>();
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            IEnumerable<OrderHistoryViewModel> orders = cpr.GetOrders().Where(o => o.deliveryOpt == "Delivery" && o.requiredDate==DateTime.Today);
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
        public ActionResult AddCustomer(string firstName, string lastName, string address, string city, string state, string zip, string phone, string cell, bool caterer, string email)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddCustomer(firstName, lastName, address, city, state, zip, phone, cell, caterer, email);
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
            IEnumerable<OrderHistoryViewModel> ordersHistory = cpr.SearchOrders(search).Take(35);
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
            cpr.DeleteOrderDetailsById(id);
            cpr.DeleteOrderById(id);
            return null;
        }

    }
}
