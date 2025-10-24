using System.Net;
using System.Text.Json;

namespace CityExplorer.Services;

public class FrankfurterService : ApiClient, IFxService
{
    public FrankfurterService(HttpClient http) : base(http) { }

    public async Task<decimal> ConvertAsync(string from, string to, decimal amount)
    {
        if (string.Equals(from, to, StringComparison.OrdinalIgnoreCase))
            return amount; // 1:1 if its the same currency

        try
        {
           
            var doc = await GetAsync<JsonElement>($"latest?amount=1&from={from}&to={to}");
            var rate = doc.GetProperty("rates").EnumerateObject().First().Value.GetDecimal();
            return rate; // just return the rate itself

        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // no rate available
            return 0m;
        }
        catch
        {
            // other issue 
            return 0m;
        }
    }
}