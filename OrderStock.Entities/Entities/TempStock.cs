using OrderStock.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStock.Entities.Entities;

public class TempStock
{
    public int Id { get; set; }
    public int ?UserId { get; set; }
    public List<Stock> Stocks { get; set; }
    public string StockStatus { get; set; } = StockStatusEnum.WaitingStock;

    public DateTime? DateOfConfirmed { get; set; }
}
