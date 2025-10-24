using CityExplorer;
using CityExplorer.Components;
using CityExplorer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<ICountryService, CountryService>(c =>
{
    c.BaseAddress = new Uri("https://restcountries.com/v3.1/");
    c.Timeout = TimeSpan.FromSeconds(20);
});

builder.Services.AddHttpClient<IWeatherService, OpenMeteoService>(c =>
{
    c.BaseAddress = new Uri("https://api.open-meteo.com/v1/");
    c.Timeout = TimeSpan.FromSeconds(20);
});

builder.Services.AddHttpClient<IFxService, FrankfurterService>(c =>
{
    c.BaseAddress = new Uri("https://api.frankfurter.app/");
    c.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddScoped<AppState>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Nice-to-have redirect
app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/countries");
    return Task.CompletedTask;
});

app.Run();