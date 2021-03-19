using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockApi.Data;
using StockApi.Models;
using StockApi.Services;

namespace StockApi.Controllers
{
    [ApiController]
    [Route("api/stock/{symbol}")]
    public class StockController : ControllerBase
    {
        private readonly IContext _context;
        private readonly ICreateStockSymbolService _createStockSymbolService;

        public StockController(IContext context, ICreateStockSymbolService createStockSymbolService)
        {
            _context = context;
            _createStockSymbolService = createStockSymbolService;
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetStockInfo(string symbol)
        {
            var result = await _context.AveragePrices.FirstOrDefaultAsync(x => x.Symbol == symbol);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CreateStockSymbolRequest request)
        {
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var result = await _createStockSymbolService.CreateStockSymbol(request);
            if (result.SymbolExists)
                return Conflict();

            // for result here, you may want to return a POCO to minimize the payload
            return Created($"api/stock/{request.Symbol}", result);
        }
    }
}
