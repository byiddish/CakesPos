using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CakesPos.Data;

namespace CakesPos
{
    public class OrdersViewModel
    {
        public IEnumerable<Product> products { get; set; }
        public IEnumerable<Category> categories { get; set; }
    }
}