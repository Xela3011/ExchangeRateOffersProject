using Core.Application.Interfaces;
using Core.Application.Models;
using System.Net.Http.Json;

namespace Core.Infrastructure.Providers
{
    public sealed class JsonProvider2 : IExchangeRateProvider
    {
        private readonly HttpClient http;
        private readonly string endpoint; //api/json1

        public JsonProvider2(HttpClient httpClient, string endpoint)
        {
            http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        private sealed record Api3Request(object exchange);
        private sealed record Api3Response(int statusCode, string message, Api3Data? data);
        private sealed record Api3Data(decimal total);


        public async Task<ExchangeResult> GetRateAsync(ExchangeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var payload = new Api3Request(new
                {
                    sourceCurrency = request.SourceCurrency,
                    targetCurrency = request.TargetCurrency,
                    quantity = request.Amount
                });
                using var resp = await http.PostAsJsonAsync(endpoint, payload, cancellationToken).ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode) return new ExchangeResult("APIJSON2", null);

                var body = await resp.Content.ReadFromJsonAsync<Api3Response>(cancellationToken: cancellationToken).ConfigureAwait(false);
                if (body?.data is null) return new ExchangeResult("APIJSON2", null);

                return new ExchangeResult("APIJSON2", body.data.total);
            }
            catch
            {
                return new ExchangeResult("APIJSON2", null);
            }
        }
    }
}
