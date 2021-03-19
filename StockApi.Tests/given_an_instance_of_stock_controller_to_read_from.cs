using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using StockApi.Controllers;
using StockApi.Data;
using StockApi.Services;
using Xunit;

namespace StockApi.Tests
{
    public class given_an_instance_of_stock_controller_to_read_from
    {
        [Fact]
        public void assert_if_a_symbol_is_not_found_a_not_found_result_is_returned()
        {
            // arrange
            var contextMock = new Mock<IContext>();
            var symbols = new List<StockAveragePrice>()
            {
                new() {Symbol = "NotFound"}
            };
            var symbolsMock = symbols.AsQueryable().BuildMockDbSet();
            contextMock.Setup(x => x.AveragePrices).Returns(symbolsMock.Object);
            
            var controller = new StockController(
                contextMock.Object,
                new Mock<ICreateStockSymbolService>().Object);
            const string symbol = "TEST";
            
            // act
            var result = controller.GetStockInfo(symbol).GetAwaiter().GetResult();
            
            // assert
            var statusResult = result as StatusCodeResult;
            Assert.NotNull(statusResult);
            Assert.Equal((int)HttpStatusCode.NotFound, statusResult.StatusCode);
        }

        [Fact]
        public void assert_if_a_symbol_exists_its_data_is_returned()
        {
            // arrange
            const string symbol = "TEST";
            var contextMock = new Mock<IContext>();
            var symbols = new List<StockAveragePrice>()
            {
                new StockAveragePrice() {Symbol = symbol}
            };
            var symbolsMock = symbols.AsQueryable().BuildMockDbSet();
            contextMock.Setup(x => x.AveragePrices).Returns(symbolsMock.Object);
            
            var controller = new StockController(
                contextMock.Object,
                new Mock<ICreateStockSymbolService>().Object);

            // act
            var result = controller.GetStockInfo(symbol).GetAwaiter().GetResult();
            
            // assert
            var objectResult = result as ObjectResult;
            Assert.NotNull(objectResult);

            var stockInfo = objectResult.Value as StockAveragePrice;
            Assert.NotNull(stockInfo);
            Assert.Equal(symbol, stockInfo.Symbol);
        }
    }
}