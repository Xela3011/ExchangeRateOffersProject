using Core.Application.Models;
using Core.Infrastructure.Providers;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace Test
{
    public class ApiProvidersTests
    {
        [Fact]
        public async Task Api1Provider_ParsesRateAndCalculatesTotal()
        {
            var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"rate\":0.5}", Encoding.UTF8, "application/json")
            });
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://fake/") };
            var provider = new JsonProvider1(client, "convert");

            var res = await provider.GetRateAsync(new ExchangeRequest("USD", "EUR", 100m), CancellationToken.None);

            Assert.Equal("APIJSON1", res.Provider);
            Assert.Equal(50m, res.ConvertedAmount);
        }

        [Fact]
        public async Task Api2Provider_ParsesXmlResult()
        {
            var xml = new XDocument(new XElement("XML", new XElement("Result", 321.99m))).ToString();
            var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(xml, Encoding.UTF8, "application/xml")
            });
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://fake/") };
            var provider = new XmlProvider(client, "convert");

            var res = await provider.GetRateAsync(new ExchangeRequest("USD", "EUR", 100m), CancellationToken.None);

            Assert.Equal("APIXML", res.Provider);
            Assert.Equal(321.99m, res.ConvertedAmount);
        }

        [Fact]
        public async Task Api3Provider_ReadsNestedJsonTotal()
        {
            var payload = JsonSerializer.Serialize(new { statusCode = 200, message = "ok", data = new { total = 777.7m } });
            var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            });
            var client = new HttpClient(handler) { BaseAddress = new Uri("https://fake/") };
            var provider = new JsonProvider2(client, "convert");

            var res = await provider.GetRateAsync(new ExchangeRequest("USD", "EUR", 100m), CancellationToken.None);

            Assert.Equal("APIJSON2", res.Provider);
            Assert.Equal(777.7m, res.ConvertedAmount);
        }

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> responder;
            public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) => this.responder = responder;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
                => Task.FromResult(responder(request));
        }
    }
}
