using Core.Application.Interfaces;
using Core.Application.Models;
using Core.Application.Services;
using Moq;

namespace Test
{
    public class ExchangeRateServiceTests
    {
        [Fact]
        public async Task ReturnsBestOfferAmongProviders()
        {
            var mock1 = new Mock<IExchangeRateProvider>();
            var mock2 = new Mock<IExchangeRateProvider>();
            var mock3 = new Mock<IExchangeRateProvider>();

            mock1.Setup(m => m.GetRateAsync(It.IsAny<ExchangeRequest>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new ExchangeResult("P1", 900m));
            mock2.Setup(m => m.GetRateAsync(It.IsAny<ExchangeRequest>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new ExchangeResult("P2", 920m));
            mock3.Setup(m => m.GetRateAsync(It.IsAny<ExchangeRequest>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new ExchangeResult("P3", 880m));

            var svc = new ExchangeRateService(new[] { mock1.Object, mock2.Object, mock3.Object });

            var res = await svc.GetBestOfferAsync(new ExchangeRequest("USD", "EUR", 100m), CancellationToken.None);

            Assert.NotNull(res);
            Assert.Equal("P2", res!.Provider);
            Assert.Equal(920m, res.ConvertedAmount);
        }

        [Fact]
        public async Task IgnoresNullResponsesAndReturnsNullIfNoValid()
        {
            var mock1 = new Mock<IExchangeRateProvider>();
            mock1.Setup(m => m.GetRateAsync(It.IsAny<ExchangeRequest>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new ExchangeResult("P1", null));

            var svc = new ExchangeRateService(new[] { mock1.Object });

            var res = await svc.GetBestOfferAsync(new ExchangeRequest("USD", "EUR", 100m), CancellationToken.None);
            Assert.Null(res);
        }

        [Fact]
        public async Task HandlesProviderThrowingWithoutCrashing()
        {
            var ok = new Mock<IExchangeRateProvider>();
            ok.Setup(m => m.GetRateAsync(It.IsAny<ExchangeRequest>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(new ExchangeResult("OK", 10m));

            var boom = new Mock<IExchangeRateProvider>();
            boom.Setup(m => m.GetRateAsync(It.IsAny<ExchangeRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("fail"));

            var svc = new ExchangeRateService(new[] { ok.Object, boom.Object });

            var res = await svc.GetBestOfferAsync(new ExchangeRequest("USD", "EUR", 1m), CancellationToken.None);
            Assert.NotNull(res);
            Assert.Equal("OK", res!.Provider);
            Assert.Equal(10m, res.ConvertedAmount);
        }
    }
}
