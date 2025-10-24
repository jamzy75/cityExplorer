using System.Net.Http;
using System.Net.Http.Json;           
using System.Text.Json;

namespace CityExplorer.Services;

public abstract class ApiClient
{
    protected readonly HttpClient Http;
    protected static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    protected ApiClient(HttpClient http) => Http = http;

    protected async Task<T> GetAsync<T>(string url)
    {
        var data = await Http.GetFromJsonAsync<T>(url, Json);
        if (data is null) throw new InvalidOperationException("Empty API response");
        return data;
    }
}