using Microsoft.Extensions.DependencyInjection;

namespace AutoPay.API.Services.Configuration;

public static class AppConfigurationExtensions
{
    public static void AddAppConfiguration(this IServiceCollection services, Func<IAppConfigurationSerializer> serializerProvider)
    {
        var serializer = serializerProvider?.Invoke();

        if (serializer is null)
        {
            throw new ArgumentNullException(nameof(IAppConfigurationSerializer));
        }
        
        services.AddSingleton<IAppConfiguration, AppConfiguration>(provider => new AppConfiguration(serializer));
    }
}