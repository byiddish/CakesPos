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
            ovm.products = cpr.GetAllProducts();
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
            IEnumerable<Product> products= cpr.GetProductsByCategory(categoryId);
            return Json(products);
        }

    }
}
