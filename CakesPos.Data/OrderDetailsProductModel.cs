using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakesPos.Data
{
    public class OrderDetailsProductModel
    {
       public  int productId { get; set; }
       public string productName { get; set; }
       public decimal unitPrice { get; set; }
       public int quantity { get; set; }
    }
}
