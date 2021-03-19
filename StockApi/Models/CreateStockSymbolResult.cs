using StockApi.Data;

namespace StockApi.Models
{
    public class CreateStockSymbolResult
    {
        public bool SymbolExists { get; set; }
        
        // keeping the example brief so reusing a class which already exists for the context
        public StockAveragePrice StockInstance { get; set; }
    }
}