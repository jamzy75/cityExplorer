using CityExplorer.Models;

namespace CityExplorer.Services;

public interface ICountryService
{
    Task<List<Country>> GetAllAsync();
    Task<Country?> GetByCodeAsync(string code);
}