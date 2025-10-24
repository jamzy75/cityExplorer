using CityExplorer.Models;

namespace CityExplorer.Services;

public interface IWeatherService
{
    Task<CityWeather?> GetWeatherAsync(double lat, double lon, string timezone = "auto");
}