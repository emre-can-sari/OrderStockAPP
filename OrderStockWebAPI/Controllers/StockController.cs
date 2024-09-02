using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderStock.Business.Services;
using OrderStock.Entities.Dtos;
using OrderStock.Entities.Entities;
using OrderStock.Entities.Enums;

namespace OrderStockWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly StockService _stockService;

    public StockController(StockService stockService)
    {
         _stockService = stockService;
    }

    [HttpGet]
    [Authorize(Roles = EnumStringRoles.User)]
    public ActionResult<List<Stock>> GetAllStocks()
    {
        var stocks = _stockService.GetAllStock();
        return Ok(stocks);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = EnumStringRoles.User)]
    public ActionResult<Stock> GetStockById(int id)
    {
        var stock = _stockService.GetStockById(id);
        if (stock == null)
        {
            return NotFound();
        }
        return Ok(stock);
    }
    
    [HttpPost]
    [Authorize(Roles = EnumStringRoles.Admin)]
    public ActionResult<Stock> AddStock([FromBody] StockDTO stockDto)
    {
        if (stockDto == null)
        {
            return BadRequest("Stock data is null");
        }

        var stock = _stockService.AddStock(stockDto);
        return CreatedAtAction(nameof(GetStockById), new { id = stock.Id }, stock);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = EnumStringRoles.Admin)]
    public ActionResult<Stock> UpdateStockQuantity(int id, [FromBody] StockDTO stockDto)
    {
        if (stockDto == null)
        {
            return BadRequest("Stock data is null");
        }

        var updatedStock = _stockService.UpdateStockQuantity(id, stockDto);
        if (updatedStock == null)
        {
            return NotFound();
        }
        return Ok(updatedStock);
    }
}
