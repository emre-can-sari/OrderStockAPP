using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderStock.Business.Services;
using OrderStock.Entities.Dtos;
using OrderStock.Entities.Entities;
using OrderStock.Entities.Enums;
using SQLitePCL;

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
    [Authorize(Roles = EnumStringRoles.Admin + "," + EnumStringRoles.User)]
    public ActionResult<List<Stock>> GetAllStocks()
    {
        var stocks = _stockService.GetAllStock();
        return Ok(stocks);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = EnumStringRoles.Admin + "," + EnumStringRoles.User)]
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
    public IActionResult AddStocks([FromBody] IEnumerable<StockDTO> stockDtos)
    {
        if (stockDtos == null || !stockDtos.Any())
        {
            return BadRequest("No stock data provided.");
        }

        try
        {
            var addedStocks = _stockService.AddStocks(stockDtos);

            return Ok(addedStocks); 
        }
        catch (Exception ex)
        {
            
            return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
        }
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
