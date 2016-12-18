using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakesPos.Data
{
    public class EditOrdersViewModel
    {
        public IEnumerable<Product> products { get; set; }
        public IEnumerable<Category> categories { get; set; }
        //public Order order { get; set; }
        //public IEnumerable<OrderDetail> orderDetails { get; set; }
        //public Customer customer { get; set; }
        public OrderDetailsViewModel orderDetails { get; set; }
        public IEnumerable<InventoryViewModel> productAvailability { get; set; }
    }
}
