using Core.Application.Interfaces;
using Core.Application.Models;

namespace Core.Application.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IEnumerable<IExchangeRateProvider> providers;

        public ExchangeRateService(IEnumerable<IExchangeRateProvider> providers)
        {
            this.providers = providers;
        }

        public async Task<ExchangeResult?> GetBestOfferAsync(ExchangeRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var tasks = providers.Select(async p =>
            {
                try
                {
                    return await p.GetRateAsync(request, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    // Provider should never throw; return a null result to be ignored
                    return new ExchangeResult(p.GetType().Name, null);
                }
            });

            var results = await Task.WhenAll(tasks).ConfigureAwait(false);

            var valid = results.Where(r => r.ConvertedAmount.HasValue).ToArray();
            if (valid.Length == 0) return null;

            // return the result with the highest ConvertedAmount
            return valid.OrderByDescending(r => r.ConvertedAmount).First();
        }
    }
}
