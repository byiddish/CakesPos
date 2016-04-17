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
            return View();
        }

        public ActionResult Admin()
        {
            return View();
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
        public ActionResult AddCustomer(string firstName, string lastName, string address, string city, string state, string zip, string phone, bool caterer)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddCustomer(firstName, lastName, address, city, state, zip, phone, caterer);
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
        public ActionResult AddOrder(int customerId, DateTime requiredDate, string deliveryOpt, string deliveryFirstName, string deliveryLastName, string deliveryAddress, string deliveryCity, string deliveryState, string deliveryZip, string phone, string creditCard, string expiration, string securityCode, string paymentMethod, decimal discount)
        {
            DateTime dateTime = DateTime.Now;
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddOrder(customerId, dateTime, requiredDate, deliveryOpt, deliveryFirstName, deliveryLastName, deliveryAddress, deliveryCity, deliveryState, deliveryZip, phone, creditCard, expiration, securityCode, paymentMethod, discount);
            return RedirectToAction("Admin");
        }

        [HttpPost]
        public ActionResult AddOrderDetails(int orderId, int productId, decimal unitPrice, int quantity)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddOrderDetails(orderId, productId, unitPrice, quantity);
            return RedirectToAction("Order");
        }
    }
}
