using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApi.Controllers;
using StockApi.Services;
using Xunit;

namespace StockApi.Tests
{
    public class given_an_instance_of_stock_controller_to_read_from
    {
        [Fact]
        public void assert_a_call_to_get_stock_info_is_made_with_the_given_symbol()
        {
            // arrange
            var getStockInfoServiceMock = new Mock<IGetStockInfoService>();
            var controller = new StockController(
                getStockInfoServiceMock.Object,
                new Mock<ICreateStockSymbolService>().Object);
            const string symbol = "TEST";

            // act
            controller.GetStockInfo(symbol).GetAwaiter().GetResult();
            
            // assert
            getStockInfoServiceMock.Verify(x => x.GetStockInfo(symbol), Times.Once);
        }

        [Fact]
        public void assert_if_a_symbol_is_not_found_a_not_found_result_is_returned()
        {
            // arrange
            var getStockInfoServiceMock = new Mock<IGetStockInfoService>();
            getStockInfoServiceMock.Setup(x => x.GetStockInfo(It.IsAny<string>()))
                .ReturnsAsync((StockInfo)null);
            
            var controller = new StockController(
                getStockInfoServiceMock.Object,
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
            var getStockInfoServiceMock = new Mock<IGetStockInfoService>();
            getStockInfoServiceMock.Setup(x => x.GetStockInfo(symbol))
                .ReturnsAsync(new StockInfo() { Symbol = symbol });
            var controller = new StockController(
                getStockInfoServiceMock.Object,
                new Mock<ICreateStockSymbolService>().Object);
            
            // act
            var result = controller.GetStockInfo(symbol).GetAwaiter().GetResult();
            
            // assert
            var objectResult = result as ObjectResult;
            Assert.NotNull(objectResult);

            var stockInfo = objectResult.Value as StockInfo;
            Assert.NotNull(stockInfo);
            Assert.Equal(symbol, stockInfo.Symbol);
        }
    }
}