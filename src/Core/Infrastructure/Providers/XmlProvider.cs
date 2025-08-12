using Core.Application.Interfaces;
using Core.Application.Models;
using System.Text;
using System.Xml.Linq;

namespace Core.Infrastructure.Providers
{
    public sealed class XmlProvider : IExchangeRateProvider
    {
        private readonly HttpClient http;
        private readonly string endpoint; //api/xml

        public XmlProvider(HttpClient httpClient, string endpoint)
        {
            http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        public async Task<ExchangeResult> GetRateAsync(ExchangeRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Build XML request: <XML><From/><To/><Amount/></XML>
                var doc = new XDocument(
                    new XElement("XML",
                        new XElement("From", request.SourceCurrency),
                        new XElement("To", request.TargetCurrency),
                        new XElement("Amount", request.Amount)
                    )
                );

                var content = new StringContent(doc.ToString(), Encoding.UTF8, "application/xml");
                using var resp = await http.PostAsync(endpoint, content, cancellationToken).ConfigureAwait(false);
                if (!resp.IsSuccessStatusCode) return new ExchangeResult("APIXML", null);

                var respStr = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                var respDoc = XDocument.Parse(respStr);

                // Response format: <XML><Result/> or similar (we try to be resilient)
                var resultElement = respDoc.Descendants("Result").FirstOrDefault();
                if (resultElement is null) return new ExchangeResult("APIXML", null);

                if (decimal.TryParse(resultElement.Value, out var total))
                    return new ExchangeResult("APIXML", total);

                return new ExchangeResult("APIXML", null);
            }
            catch
            {
                return new ExchangeResult("APIXML", null);
            }
        }
    }
}
