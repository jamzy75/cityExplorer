namespace CityExplorer.Models;

public enum Region { Africa, Americas, Asia, Europe, Oceania, Antarctic, Other }
public enum ConvertAmount { Ten = 10, Fifty = 50, Hundred = 100, TwoHundred = 200 }

public record Country(
    string Code,
    string Name,
    Region Region,
    string? Capital,
    long Population,
    double Area,
    string FlagPng,
    double? Lat,
    double? Lng,
    string CurrencyCode,
    string CurrencyName,
    List<Neighbour> Neighbours
);

public record Neighbour(string Code, string Name);

public record CityWeather(
    double Latitude,
    double Longitude,
    string Timezone,
    WeatherCurrent Current,
    List<WeatherDaily> Daily
);

public record WeatherCurrent(DateTimeOffset Time, double TemperatureC, string Description);
public record WeatherDaily(DateOnly Date, double MinC, double MaxC, DateTimeOffset Sunrise, DateTimeOffset Sunset);