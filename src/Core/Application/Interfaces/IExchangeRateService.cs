using Core.Application.Models;

namespace Core.Application.Interfaces
{
    public interface IExchangeRateService
    {
        Task<ExchangeResult?> GetBestOfferAsync(ExchangeRequest request, CancellationToken cancellationToken);
    }
}
