using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakesPos.Data
{
    public class ProductDetailsViewModel
    {
        public int orderDetailId { get; set; }
        public int orderId { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public decimal price { get; set; }
        public int inStock { get; set; }

    }
}
