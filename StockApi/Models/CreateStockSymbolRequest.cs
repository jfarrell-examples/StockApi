namespace StockApi.Models
{
    public class CreateStockSymbolRequest
    {
        public string Symbol { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}