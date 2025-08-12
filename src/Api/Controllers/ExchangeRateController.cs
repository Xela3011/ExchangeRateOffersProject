using Core.Application.Interfaces;
using Core.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/exchangeRate")]
    public class ExchangeRateController : Controller
    {
        private readonly ILogger<ExchangeRateController> logger;
        private readonly IExchangeRateService exchangeRateService;

        public ExchangeRateController(ILogger<ExchangeRateController> logger, IExchangeRateService exchangeRateService)
        {
            this.logger = logger;
            this.exchangeRateService = exchangeRateService;
        }

        [HttpGet("best-offer")]
        public async Task<IActionResult> GetBestOffer([FromQuery] string sourceCurrency, [FromQuery] string targetCurrency, [FromQuery] decimal amount, CancellationToken cancellationToken)
        {
            var request = new ExchangeRequest(sourceCurrency, targetCurrency, amount);
            var result = await exchangeRateService.GetBestOfferAsync(request, cancellationToken);
            if (result is null)
            {
                logger.LogWarning("No valid offers found for {Source} to {Target}", sourceCurrency, targetCurrency);
                return NotFound(new { message = "No valid offers found." });
            }

            return Ok(result);
        }
    }
}
