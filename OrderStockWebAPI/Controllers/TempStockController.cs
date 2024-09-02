using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderStock.Business.Services;
using OrderStock.Entities.Dtos;
using OrderStock.Entities.Entities;
using OrderStock.Entities.Enums;

namespace OrderStockWebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TempStockController : ControllerBase
{
    private readonly TempStockService _tempStockService;

    public TempStockController(TempStockService tempStockService)
    {
        _tempStockService = tempStockService;
    }

    [HttpGet("confirmed")]
    [Authorize(Roles = EnumStringRoles.User)]
    public ActionResult<List<TempStock>> GetConfirmedStock()
    {
        var stocks = _tempStockService.GetConfirmedStock();
        return Ok(stocks);
    }

    [HttpGet("waiting")]
    [Authorize(Roles = EnumStringRoles.User)]
    public ActionResult<List<TempStock>> GetWaitingStock()
    {
        var stocks = _tempStockService.GetWaitingStock();
        return Ok(stocks);
    }
    [HttpGet("{id}")]
    [Authorize(Roles = EnumStringRoles.User)]
    public ActionResult<TempStock> GetTempStockById(int id)
    {
        var tempStock = _tempStockService.GetTempStockById(id);
        if (tempStock == null)
        {
            return NotFound();
        }
        return Ok(tempStock);
    }

    [HttpPost]
    [Authorize(Roles = EnumStringRoles.Admin)]
    public ActionResult<TempStock> AddTempStock([FromBody] TempStockDTO tempStockDTO)
    {
        if (tempStockDTO == null)
        {
            return BadRequest("Stock data is null");
        }

        try
        {
            var tempStock = _tempStockService.AddTempStock(tempStockDTO);
            return CreatedAtAction(nameof(GetTempStockById), new { id = tempStock.Id }, tempStock);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("confirm/{tempStockId}")]
    [Authorize(Roles = EnumStringRoles.Admin)]
    public ActionResult<List<Stock>> ConfirmStock(int tempStockId)
    {
        try
        {
            var stock = _tempStockService.ConfirmedStock(tempStockId);
            return Ok(stock);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }


}
