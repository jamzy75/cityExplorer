using System.Text.Json;
using System.Linq;
using CityExplorer.Models;

namespace CityExplorer.Services;

public class CountryService : ApiClient, ICountryService
{
    public CountryService(HttpClient http) : base(http) { }

    public async Task<List<Country>> GetAllAsync()
    {
        var doc = await GetAsync<JsonElement>(
            "all?fields=name,cca3,region,capital,population,area,flags,latlng,currencies,borders");

        return doc.EnumerateArray()
                  .Select(ParseCountry)
                  .OrderBy(c => c.Name)
                  .ToList();
    }

    public async Task<Country?> GetByCodeAsync(string code)
    {
        try
        {
            var doc = await GetAsync<JsonElement>($"alpha/{code}");
            var node = doc.ValueKind == JsonValueKind.Array && doc.GetArrayLength() > 0
                ? doc[0]
                : doc;
            return ParseCountry(node);
        }
        catch
        {
            return null;
        }
    }


   private static Country ParseCountry(JsonElement e)
{
    string code = e.GetProperty("cca3").GetString()!;
    string name = e.GetProperty("name").GetProperty("common").GetString()!;

    string regionStr = e.TryGetProperty("region", out var r) ? r.GetString() ?? "Other" : "Other";
    Region region = regionStr switch
    {
        "Africa" => Region.Africa,
        "Americas" => Region.Americas,
        "Asia" => Region.Asia,
        "Europe" => Region.Europe,
        "Oceania" => Region.Oceania,
        "Antarctic" => Region.Antarctic,
        _ => Region.Other
    };

    string? capital = null;
    if (e.TryGetProperty("capital", out var cap) &&
        cap.ValueKind == JsonValueKind.Array && cap.GetArrayLength() > 0)
        capital = cap[0].GetString();

    long population = e.TryGetProperty("population", out var p) ? p.GetInt64() : 0;
    double area = e.TryGetProperty("area", out var a) ? a.GetDouble() : 0;

    string flagPng = "";
    if (e.TryGetProperty("flags", out var fl) &&
        fl.ValueKind == JsonValueKind.Object &&
        fl.TryGetProperty("png", out var fp) && fp.ValueKind == JsonValueKind.String)
        flagPng = fp.GetString() ?? "";

    double? lat = null, lng = null;
    if (e.TryGetProperty("latlng", out var ll) &&
        ll.ValueKind == JsonValueKind.Array && ll.GetArrayLength() >= 2)
    {
        lat = ll[0].GetDouble();
        lng = ll[1].GetDouble();
    }

   
    string currencyCode = "", currencyName = "";
    if (e.TryGetProperty("currencies", out var cur) &&
        cur.ValueKind == JsonValueKind.Object)
    {
        foreach (var prop in cur.EnumerateObject())
        {
            currencyCode = prop.Name ?? "";
            currencyName = prop.Value.TryGetProperty("name", out var nm)
                ? nm.GetString() ?? ""
                : "";
            break; 
        }
    }

    var neighbours = new List<Neighbour>();
    if (e.TryGetProperty("borders", out var br) &&
        br.ValueKind == JsonValueKind.Array)
    {
        neighbours = br.EnumerateArray()
                       .Select(b => new Neighbour(b.GetString() ?? "", b.GetString() ?? ""))
                       .ToList();
    }

    return new Country(code, name, region, capital, population, area,
                       flagPng, lat, lng, currencyCode, currencyName, neighbours);
}

}
