using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MockQueryable.Moq;
using Moq;
using StockApi.Data;
using StockApi.Models;
using StockApi.Services;
using Xunit;

namespace StockApi.Tests
{
    public class given_an_instance_of_create_stock_symbol_service
    {
        [Fact]
        public void assert_if_the_symbol_already_exists_the_result_indicates_the_symbol_exists()
        {
            // arrange
            const string symbol = "TEST";
            var context = new Mock<IContext>();
            var symbols = new List<StockAveragePrice> {new() {Symbol = symbol}};
            context.Setup(x => x.AveragePrices).Returns(symbols.AsQueryable().BuildMockDbSet().Object);

            var service = new CreateStockSymbolService(context.Object);
            
            // act
            var result = service.CreateStockSymbol(new CreateStockSymbolRequest() {Symbol = symbol})
                .GetAwaiter()
                .GetResult();
            
            // assert
            Assert.True(result.SymbolExists);
        }

        [Fact]
        public void assert_that_for_a_unique_symbol_an_instance_is_added_to_the_context()
        {
            // arrange
            const string symbol = "NEW_SYMBOL";
            var context = new Mock<IContext>();
            var symbols = new List<StockAveragePrice> {new() {Symbol = "TEST"}};
            var symbolSetMock = symbols.AsQueryable().BuildMockDbSet();
            context.Setup(x => x.AveragePrices).Returns(symbolSetMock.Object);

            var service = new CreateStockSymbolService(context.Object);
            
            // act
            service.CreateStockSymbol(new CreateStockSymbolRequest() {Symbol = symbol})
                .GetAwaiter()
                .GetResult();
            
            // assert
            symbolSetMock.Verify(x => x.AddAsync(It.Is<StockAveragePrice>(
                x1 => x1.Symbol == symbol), default(CancellationToken)),
                Times.Once);
        }

        [Fact]
        public void assert_that_for_a_unique_symbol_save_changes_is_called_one_time()
        {
            // arrange
            const string symbol = "NEW_SYMBOL";
            var contextMock = new Mock<IContext>();
            var symbols = new List<StockAveragePrice> {new() {Symbol = "TEST"}};
            var symbolSetMock = symbols.AsQueryable().BuildMockDbSet();
            contextMock.Setup(x => x.AveragePrices).Returns(symbolSetMock.Object);

            var service = new CreateStockSymbolService(contextMock.Object);
            
            // act
            service.CreateStockSymbol(new CreateStockSymbolRequest() {Symbol = symbol})
                .GetAwaiter()
                .GetResult();
            
            // assert
            contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public void assert_that_on_successful_save_a_non_null_instance_is_returned_with_symbol()
        {
            // arrange
            const string symbol = "NEW_SYMBOL";
            var contextMock = new Mock<IContext>();
            var symbols = new List<StockAveragePrice> {new() {Symbol = "TEST"}};
            var symbolSetMock = symbols.AsQueryable().BuildMockDbSet();
            contextMock.Setup(x => x.AveragePrices).Returns(symbolSetMock.Object);

            var service = new CreateStockSymbolService(contextMock.Object);
            
            // act
            var result = service.CreateStockSymbol(new CreateStockSymbolRequest() {Symbol = symbol})
                .GetAwaiter()
                .GetResult();
            
            // assert
            Assert.NotNull(result.StockInstance);
            Assert.Equal(symbol, result.StockInstance.Symbol);
        }
    }
}