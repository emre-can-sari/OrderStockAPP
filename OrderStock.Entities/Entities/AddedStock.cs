using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStock.Entities.Entities;

public class AddedStock
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Quantity { get; set; }
}
