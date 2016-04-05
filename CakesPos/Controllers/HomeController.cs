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
            return View();
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

        public ActionResult AddNewProduct(string productName, decimal price, int inStock, string image, int categoryId)
        {
            CakesPosRepository cpr = new CakesPosRepository(_connectionString);
            cpr.AddProduct(productName, price, inStock, image, categoryId);
            return RedirectToAction("Admin");
        }

    }
}
