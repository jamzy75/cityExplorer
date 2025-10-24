namespace CityExplorer.Services;

public interface IFxService
{
    Task<decimal> ConvertAsync(string from, string to, decimal amount);
}