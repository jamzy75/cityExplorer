using CityExplorer.Models;
using CityExplorer.Services;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CityExplorer.Tests;

public class OpenMeteoServiceTests
{
    [Fact]
    public async Task GetWeatherAsync_ReturnsCityWeather()
    {
        var json = """
                   {
                     "latitude": 53.3,
                     "longitude": -6.2,
                     "timezone": "Europe/Dublin",
                     "current": {
                       "time": "2024-05-01T12:00Z",
                       "temperature_2m": 15.5,
                       "weather_code": 3
                     },
                     "daily": {
                       "time": ["2024-05-01"],
                       "temperature_2m_min": [10],
                       "temperature_2m_max": [18],
                       "sunrise": ["2024-05-01T05:00Z"],
                       "sunset": ["2024-05-01T21:00Z"]
                     }
                   }
                   """;

        var handler = new FakeHandler(json);
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://fake/") };
        var svc = new OpenMeteoService(client);

        var result = await svc.GetWeatherAsync(53.3, -6.2);

        Assert.NotNull(result);
        Assert.Equal("Europe/Dublin", result.Timezone);
        Assert.Equal("Overcast", result.Current.Description);
        Assert.Single(result.Daily);
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