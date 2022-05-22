namespace AutoPay.API.Services.Configuration;

public interface IAppConfiguration
{
    string GetValue(string key);
    string GetOrSetDefault(string key, Func<string> defaultValue);
    void SetValue(string key, string value);
    void Load();
    void Save();
}