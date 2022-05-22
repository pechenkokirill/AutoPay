using System.Diagnostics;

namespace AutoPay.API.Services.Configuration;

public class AppConfiguration : IAppConfiguration
{
    private readonly IAppConfigurationSerializer _serializer;
    private IDictionary<string, string> _config;

    public AppConfiguration(IAppConfigurationSerializer serializer)
    {
        _serializer = serializer;
        _config = _serializer.Deserialize();
    }

    public string GetValue(string key)
    {
        return _config.ContainsKey(key) ? _config[key] : null;
    }

    public string GetOrSetDefault(string key, Func<string> defaultValue)
    {
        Debug.Assert(defaultValue is not null);
        var value = GetValue(key);

        if (string.IsNullOrWhiteSpace(value))
        {
            value = defaultValue.Invoke();
            SetValue(key, value);
            Save();
        }

        return value;
    }

    public void SetValue(string key, string value)
    {
        _config[key] = value;
    }

    public void Load()
    {
        _config = _serializer.Deserialize();
    }

    public void Save()
    {
        _serializer.Serialize(_config);  
    }
}