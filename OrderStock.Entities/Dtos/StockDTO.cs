using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStock.Entities.Dtos
{
    public class StockDTO
    {
        public string Name { get; set; }
        public decimal Quantity { get; set; }
    }
}
