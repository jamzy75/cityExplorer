using CityExplorer.Models;
using CityExplorer.Services;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace CityExplorer.Tests;

public class CountryServiceTests
{
    [Fact]
    public async Task GetAllAsync_ParsesCountries()
    {
        var json = """
                   [
                     {
                       "cca3":"IRL",
                       "name":{"common":"Ireland"},
                       "region":"Europe",
                       "capital":["Dublin"],
                       "population":5000000,
                       "area":70000,
                       "flags":{"png":"https://flag"},
                       "latlng":[53.3,-6.2],
                       "currencies":{"EUR":{"name":"Euro"}},
                       "borders":["GBR"]
                     }
                   ]
                   """;

        var handler = new FakeHandler(json);
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://fake/") };
        var service = new CountryService(client);

        var result = await service.GetAllAsync();

        Assert.Single(result);
        var c = result[0];
        Assert.Equal("Ireland", c.Name);
        Assert.Equal(Region.Europe, c.Region);
        Assert.Equal("EUR", c.CurrencyCode);
        Assert.Equal("Euro", c.CurrencyName);
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