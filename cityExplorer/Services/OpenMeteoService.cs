using System.Text.Json;
using CityExplorer.Models;

namespace CityExplorer.Services;

public class OpenMeteoService : ApiClient, IWeatherService
{
    public OpenMeteoService(HttpClient http) : base(http) { }

    public async Task<CityWeather?> GetWeatherAsync(double lat, double lon, string timezone = "auto")
    {
        var url =
            $"forecast?latitude={lat:0.####}&longitude={lon:0.####}" +
            "&current=temperature_2m,weather_code" +
            "&daily=weather_code,temperature_2m_max,temperature_2m_min,sunrise,sunset" +
            $"&timezone={timezone}";

        var doc = await GetAsync<JsonElement>(url);

        double latitude  = doc.GetProperty("latitude").GetDouble();
        double longitude = doc.GetProperty("longitude").GetDouble();
        string tz        = doc.TryGetProperty("timezone", out var tzEl) ? tzEl.GetString() ?? "UTC" : "UTC";

        var cur = doc.GetProperty("current");
        var curTime = cur.GetProperty("time").GetDateTimeOffset();
        var curTemp = cur.GetProperty("temperature_2m").GetDouble();
        var curCode = cur.GetProperty("weather_code").GetInt32();

        var daily = doc.GetProperty("daily");
        var dates   = daily.GetProperty("time").EnumerateArray().Select(x => DateOnly.FromDateTime(x.GetDateTime())).ToArray();
        var mins    = daily.GetProperty("temperature_2m_min").EnumerateArray().Select(x => x.GetDouble()).ToArray();
        var maxes   = daily.GetProperty("temperature_2m_max").EnumerateArray().Select(x => x.GetDouble()).ToArray();
        var sunrise = daily.GetProperty("sunrise").EnumerateArray().Select(x => x.GetDateTimeOffset()).ToArray();
        var sunset  = daily.GetProperty("sunset").EnumerateArray().Select(x => x.GetDateTimeOffset()).ToArray();

        var days = new List<WeatherDaily>();
        for (int i = 0; i < dates.Length; i++)
            days.Add(new WeatherDaily(dates[i], mins[i], maxes[i], sunrise[i], sunset[i]));

        return new CityWeather(latitude, longitude, tz,
            new WeatherCurrent(curTime, curTemp, WeatherCodeToText(curCode)),
            days);
    }

    static string WeatherCodeToText(int code) => code switch
    {
        0 => "Clear",
        1 or 2 => "Partly cloudy",
        3 => "Overcast",
        45 or 48 => "Fog",
        51 or 53 or 55 => "Drizzle",
        61 or 63 or 65 => "Rain",
        71 or 73 or 75 => "Snow",
        95 or 96 or 99 => "Thunderstorm",
        _ => "—"
    };
}
