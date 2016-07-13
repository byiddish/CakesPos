using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakesPos.Data
{
    public class OrderHistoryViewModel
    {
        public int id { get; set; }
        public int customerId { get; set; }
        public bool caterer { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime orderDate { get; set; }
        public DateTime requiredDate { get; set; }
        public string deliveryOpt { get; set; }
        public string paymentMethod { get; set; }
        public bool paid { get; set; }
        public decimal amount { get; set; }
        public decimal discount { get; set; }
        public Status status { get; set; }
        public decimal total { get; set; }
        public decimal balance { get; set; }
        public bool invoice { get; set; }
        public bool statement { get; set; }
        public IEnumerable<Payment> payments { get; set; }
    }
}
