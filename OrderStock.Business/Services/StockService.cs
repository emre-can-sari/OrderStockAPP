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

    public Stock AddStock(StockDTO stockDto) {
        Stock stock = new Stock
        {
            Name = stockDto.Name,
            Quantity = stockDto.Quantity
        };
        _context.Stocks.Add(stock);
        _context.SaveChanges();
        return stock;
    }

    public Stock UpdateStockQuantity(int id, StockDTO stockDto) { 
       var updatedStock = _context.Stocks.SingleOrDefault(x=> x.Id == id);
        updatedStock.Quantity = stockDto.Quantity;

        _context.Stocks.Update(updatedStock);
        _context.SaveChanges();
        return updatedStock;
    }
}
