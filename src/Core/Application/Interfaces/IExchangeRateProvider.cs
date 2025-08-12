using Core.Application.Models;

namespace Core.Application.Interfaces
{
    public interface IExchangeRateProvider
    {
        Task<ExchangeResult> GetRateAsync(ExchangeRequest request, CancellationToken cancellationToken);
    }
}
