using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockApi.Services;

namespace StockApi.Controllers
{
    [ApiController]
    [Route("api/stock/{symbol}")]
    public class StockController : ControllerBase
    {
        private readonly IGetStockInfoService _getStockInfoService;

        public StockController(IGetStockInfoService getStockInfoService)
        {
            _getStockInfoService = getStockInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStockInfo(string symbol)
        {
            var result = await _getStockInfoService.GetStockInfo(symbol);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
