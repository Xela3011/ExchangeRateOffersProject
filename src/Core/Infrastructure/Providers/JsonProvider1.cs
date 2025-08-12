using Core.Application.Interfaces;
using Core.Application.Models;
using System.Net.Http.Json;

namespace Core.Infrastructure.Providers
{
    public sealed class JsonProvider1 : IExchangeRateProvider
    {
        private readonly HttpClient http;
        private readonly string endpoint; //api/json1

        public JsonProvider1(HttpClient httpClient, string endpoint)
        {
            http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }
        private sealed record Api1Request(string from, string to, decimal value);
        private sealed record Api1Response(decimal rate);

        public async Task<ExchangeResult> GetRateAsync(ExchangeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var payload = new Api1Request(request.SourceCurrency, request.TargetCurrency, request.Amount);
                using var resp = await http.PostAsJsonAsync(endpoint, payload, cancellationToken).ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode) return new ExchangeResult("APIJSON1", null);

                var body = await resp.Content.ReadFromJsonAsync<Api1Response>(cancellationToken: cancellationToken).ConfigureAwait(false);
                if (body is null) return new ExchangeResult("APIJSON1", null);

                // API returns a rate (e.g. 0.92). ConvertedAmount = amount * rate
                return new ExchangeResult("APIJSON1", body.rate * request.Amount);
            }
            catch
            {
                return new ExchangeResult("APIJSON1", null);
            }
        }
    }
}
