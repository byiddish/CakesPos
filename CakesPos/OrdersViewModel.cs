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
        public IEnumerable<InventoryViewModel> productAvailability { get; set; }
        //public Order order { get; set; }
        //public IEnumerable<OrderDetail> orderDetails { get; set; }
        //public Customer customer { get; set; }
    }
}