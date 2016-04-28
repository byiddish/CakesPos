using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakesPos.Data
{
    public class OrderDetailsViewModel
    {
        public Customer customer { get; set; }
        public IEnumerable<OrderDetail> orderDetails { get; set; }
        public Order order { get; set; }
        public IEnumerable<Payment> payments { get; set; }
        public IEnumerable<OrderDetailsProductModel> orderedProducts { get; set; }
    }
}
