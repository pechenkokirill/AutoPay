namespace AutoPay.API.Services.Configuration;

public interface IAppConfigurationSerializer
{
    IDictionary<string, string> Deserialize();
    void Serialize(IDictionary<string, string> data);
}