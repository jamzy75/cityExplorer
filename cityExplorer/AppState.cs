namespace CityExplorer;

public class AppState
{
    public string Theme { get; private set; } = "dark";
    public event Action? Changed;

    public void SetTheme(string value)
    {
        if (Theme == value) return;
        Theme = value;
        Changed?.Invoke();
    }
}