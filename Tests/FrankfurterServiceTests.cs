using CityExplorer.Services;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CityExplorer.Tests;

public class FrankfurterServiceTests
{
    [Fact]
    public async Task ConvertAsync_ReturnsRate()
    {
        var json = """{"amount":1,"base":"EUR","date":"2024-05-01","rates":{"USD":1.08}}""";
        var handler = new FakeHandler(json);
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://fake/") };
        var svc = new FrankfurterService(client);

        var rate = await svc.ConvertAsync("EUR", "USD", 1m);

        Assert.Equal(1.08m, rate, 2); // I did this to allow rounding 
    }

    [Fact]
    public async Task ConvertAsync_SameCurrency_ReturnsAmount()
    {
        var svc = new FrankfurterService(new HttpClient(new FakeHandler("{}"))
        {
            BaseAddress = new Uri("https://fake/")
        });
        var rate = await svc.ConvertAsync("USD", "USD", 5m);
        Assert.Equal(5m, rate);
    }

    private class FakeHandler : HttpMessageHandler
    {
        private readonly string _json;
        public FakeHandler(string json) => _json = json;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage r, CancellationToken t)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_json, Encoding.UTF8, "application/json")
            });
    }
}