using OrderStock.DataAccess;
using OrderStock.Entities.Dtos;
using OrderStock.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderStock.Business.Services;

public class StockService
{
    private readonly ApplicationDbContext _context;
    public StockService(ApplicationDbContext context)
    {
        _context = context;
    }


    public List<Stock> GetAllStock()
    {
        return _context.Stocks.ToList();
    } 

    public Stock GetStockById(int id)
    {
        return _context.Stocks.SingleOrDefault(x=> x.Id == id);
    }

    public IEnumerable<Stock> AddStocks(IEnumerable<StockDTO> stockDtos)
    {
        var stocks = new List<Stock>();

        foreach (var stockDto in stockDtos)
        {
            var stock = new Stock
            {
                Name = stockDto.Name,
                Quantity = stockDto.Quantity
            };
            stocks.Add(stock);
        }

        _context.Stocks.AddRange(stocks);
        _context.SaveChanges();

        return stocks; 
    }


    public Stock UpdateStockQuantity(int id, StockDTO stockDto) { 
       var updatedStock = _context.Stocks.SingleOrDefault(x=> x.Id == id);
        updatedStock.Quantity = stockDto.Quantity;

        _context.Stocks.Update(updatedStock);
        _context.SaveChanges();
        return updatedStock;
    }
}
