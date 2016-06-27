using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakesPos.Data
{
    public class StatementsModel
    {
        public IEnumerable<OrderDetailsViewModel> Orders { get; set; }
        public Statement Statement { get; set; }
        public IEnumerable<StatementPayment> Payments { get; set; }
    }
}
