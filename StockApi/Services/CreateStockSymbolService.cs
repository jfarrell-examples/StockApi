using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using StockApi.Data;
using StockApi.Models;

namespace StockApi.Services
{
    public class CreateStockSymbolService : ICreateStockSymbolService
    {
        private readonly IContext _context;

        public CreateStockSymbolService(IContext context)
        {
            _context = context;
        }

        public async Task<CreateStockSymbolResult> CreateStockSymbol(CreateStockSymbolRequest request)
        {
            if (await _context.AveragePrices.AnyAsync(x => x.Symbol == request.Symbol))
                return new CreateStockSymbolResult() {SymbolExists = true};

            var newStock = new StockAveragePrice()
            {
                Symbol = request.Symbol,
                Total = request.CurrentPrice,
                TotalEntries = 1,
                WrittenOn = DateTime.UtcNow
            };

            await _context.AveragePrices.AddAsync(newStock);
            await _context.SaveChangesAsync();

            return new CreateStockSymbolResult() {StockInstance = newStock};
        }
    }

    public interface ICreateStockSymbolService
    {
        Task<CreateStockSymbolResult> CreateStockSymbol(CreateStockSymbolRequest request);
    }
}