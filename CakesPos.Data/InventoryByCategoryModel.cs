using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CakesPos.Data
{
    public class InventoryByCategoryModel
    {
        public IEnumerable<InventoryViewModel> inventory { get; set; }
        public IEnumerable<Category> categories { get; set; }
    }
}
