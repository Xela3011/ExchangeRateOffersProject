using Api.Controllers;
using Core.Application.Interfaces;
using Core.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test
{
    public class ExchangeRateControllerTests
    {
        [Fact]
        public async Task GetBestOffer_ReturnsOk_WhenResultExists()
        {
            var logger = new Mock<ILogger<ExchangeRateController>>();
            var svc = new Mock<IExchangeRateService>();
            svc.Setup(s => s.GetBestOfferAsync(It.IsAny<ExchangeRequest>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ExchangeResult("APIJSON1", 123.45m));

            var controller = new ExchangeRateController(logger.Object, svc.Object);

            var result = await controller.GetBestOffer("USD", "EUR", 100m, CancellationToken.None);

            var ok = Assert.IsType<OkObjectResult>(result);
            var payload = Assert.IsType<ExchangeResult>(ok.Value);
            Assert.Equal("APIJSON1", payload.Provider);
            Assert.Equal(123.45m, payload.ConvertedAmount);
        }

        [Fact]
        public async Task GetBestOffer_ReturnsNotFound_WhenNoOffers()
        {
            var logger = new Mock<ILogger<ExchangeRateController>>();
            var svc = new Mock<IExchangeRateService>();
            svc.Setup(s => s.GetBestOfferAsync(It.IsAny<ExchangeRequest>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((ExchangeResult?)null);

            var controller = new ExchangeRateController(logger.Object, svc.Object);

            var result = await controller.GetBestOffer("USD", "EUR", 100m, CancellationToken.None);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
