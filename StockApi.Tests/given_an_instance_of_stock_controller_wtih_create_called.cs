using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StockApi.Controllers;
using StockApi.Data;
using StockApi.Models;
using StockApi.Services;
using Xunit;

namespace StockApi.Tests
{
    public class given_an_instance_of_stock_controller_wtih_create_called
    {
        [Fact]
        public void assert_that_if_the_model_state_is_not_valid_a_bad_request_result_is_returned()
        {
            // arrange
            var controller = new StockController(
                new Mock<IContext>().Object,
                new Mock<ICreateStockSymbolService>().Object);

            // act
            controller.ModelState.AddModelError(string.Empty, string.Empty);
            var result = controller.Create(new CreateStockSymbolRequest()).GetAwaiter().GetResult();
            
            // act
            var statusResult = result as BadRequestObjectResult;
            Assert.NotNull(statusResult);
        }

        [Fact]
        public void assert_that_symbol_create_service_is_called_one_time_only()
        {
            // arrange
            var createStockServiceMock = new Mock<ICreateStockSymbolService>();
            createStockServiceMock.Setup(x => x.CreateStockSymbol(It.IsAny<CreateStockSymbolRequest>()))
                .ReturnsAsync(new CreateStockSymbolResult());
            
            var controller = new StockController(
                new Mock<IContext>().Object,
                createStockServiceMock.Object);
            
            // act
            controller.Create(new CreateStockSymbolRequest() { Symbol = "TEST" }).GetAwaiter().GetResult();
            
            // assert
            createStockServiceMock.Verify(
                x => x.CreateStockSymbol(It.Is<CreateStockSymbolRequest>(
                    x1 => x1.Symbol == "TEST")),
                Times.Once);
        }

        [Fact]
        public void assert_that_if_the_symbol_already_exists_a_conflict_response_code_is_returned()
        {
            // arrange
            var createStockServiceMock = new Mock<ICreateStockSymbolService>();
            createStockServiceMock.Setup(x => x.CreateStockSymbol(It.IsAny<CreateStockSymbolRequest>()))
                .ReturnsAsync(new CreateStockSymbolResult() { SymbolExists = true });
            
            var controller = new StockController(
                new Mock<IContext>().Object,
                createStockServiceMock.Object);
            
            // act
            var result = controller.Create(new CreateStockSymbolRequest()).GetAwaiter().GetResult();
            
            // assert
            var statusCodeResult = result as StatusCodeResult;
            Assert.NotNull(statusCodeResult);
            Assert.Equal(statusCodeResult.StatusCode, (int)HttpStatusCode.Conflict);
        }

        [Fact]
        public void assert_that_if_create_is_successful_a_created_result_is_returned_with_the_right_uri_identifier()
        {
            // arrange
            const string symbol = "TEST";
            var createStockServiceMock = new Mock<ICreateStockSymbolService>();
            createStockServiceMock.Setup(
                    x => x.CreateStockSymbol(It.Is<CreateStockSymbolRequest>(
                        x => x.Symbol == symbol)))
                .ReturnsAsync(new CreateStockSymbolResult());
            
            var controller = new StockController(
                new Mock<IContext>().Object,
                createStockServiceMock.Object);
            
            // act
            var result = controller.Create(new CreateStockSymbolRequest() {Symbol = symbol}).GetAwaiter().GetResult();
            
            // assert
            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.EndsWith(symbol, createdResult.Location);
        }
    }
}