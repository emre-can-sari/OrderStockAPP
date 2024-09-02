using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OrderStock.DataAccess;
using OrderStock.Entities.Dtos;
using OrderStock.Entities.Entities;
using OrderStock.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OrderStock.Business.Services;

public class TempStockService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TempStockService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public TempStock GetTempStockById(int id)
    {
        return _context.TempStocks
            .Include(x => x.Stocks)
            .SingleOrDefault(x => x.Id == id);
    }

    public List<TempStock> GetConfirmedStock()
    {
        return _context.TempStocks
            .Include(x => x.Stocks)
            .Where(x => x.StockStatus == StockStatusEnum.ConfirmedStock).ToList();
    }

    public List<TempStock> GetWaitingStock()
    {
        return _context.TempStocks
            .Include(x => x.Stocks)
            .Where(x => x.StockStatus == StockStatusEnum.WaitingStock).ToList();
    }

    public TempStock AddTempStock(TempStockDTO stockDTOs)
    {
        var tempStock = new TempStock();

        List<Stock> stockList = new List<Stock>();
        foreach (var stockDTO in stockDTOs.StockDTOs)
        {
            var stock = _context.Stocks.SingleOrDefault(x => x.Name == stockDTO.Name);

            if (stock != null)
            {
                stock.Quantity = stockDTO.Quantity;
                stockList.Add(stock);
            }
            else
            {
                throw new Exception($"Stock with name {stockDTO.Name} not found");
            }
        }
        tempStock.Stocks = stockList;
        _context.TempStocks.Add(tempStock);
        _context.SaveChanges();
        return tempStock;

    }
    public List<Stock> ConfirmedStock(int tempStockId)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            var tempStock = _context.TempStocks
                .Include(ts => ts.Stocks)
                .SingleOrDefault(x => x.Id == tempStockId);

            if (tempStock == null)
            {
                throw new Exception("TempStock not found");
            }

            var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int userId))
            {
                tempStock.StockStatus = StockStatusEnum.ConfirmedStock;
                tempStock.DateOfConfirmed = DateTime.Now;
                tempStock.UserId = userId;
                _context.TempStocks.Update(tempStock);

                foreach (var tempStockItem in tempStock.Stocks)
                {
                    var stock = _context.Stocks.SingleOrDefault(x => x.Name == tempStockItem.Name);
                    if (stock == null)
                    {
                        throw new Exception($"Stock with name {tempStockItem.Name} not found");
                    }

                    stock.Quantity += tempStockItem.Quantity;
                    _context.Stocks.Update(stock);
                }

                _context.SaveChanges();
                transaction.Commit();
                return tempStock.Stocks;
            }
            else
            {
                throw new Exception("User not found");
            }
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw;
        }
    }


    //public Stock ConfirmedStock(int tempStockId) {
    //    using var transaction = _context.Database.BeginTransaction();

    //    try
    //    {
    //        var tempStock = _context.TempStocks.SingleOrDefault(x => x.Id == tempStockId);
    //        if (tempStock == null)
    //        {
    //            throw new Exception("TempStock not found");
    //        }
    //        var claim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
    //        if (claim != null && int.TryParse(claim.Value, out int userId))
    //        {
    //            tempStock.StockStatus = StockStatusEnum.ConfirmedStock;
    //            tempStock.DateOfConfirmed = DateTime.Now;
    //            tempStock.UserId = userId;
    //            _context.TempStocks.Update(tempStock);

    //            var stock = _context.Stocks.SingleOrDefault(x => x.Name == tempStock.Stocks.Name);
    //            if (stock == null)
    //            {
    //                throw new Exception("Stock not found");
    //            }

    //            stock.Quantity += tempStock.Stocks.Quantity;
    //            _context.Stocks.Update(stock);

    //            _context.SaveChanges();

    //            transaction.Commit();
    //            return stock;
    //        }
    //        else
    //        {
    //            throw new Exception("User not found");
    //        }

    //    }
    //    catch (Exception ex)
    //    {
    //        transaction.Rollback(); 
    //        throw; 
    //    }
    //}
}