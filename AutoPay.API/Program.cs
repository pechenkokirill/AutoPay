using System.Net;
using System.Security.Policy;
using AutoPay.API.DB;
using AutoPay.API.Services.Configuration;
using Microsoft.AspNetCore;
using Microsoft.EntityFrameworkCore;

namespace AutoPay.API;

class Program
{
    private const string ConfigPath = "config.ini";

    static void Main(string[] args)
    {
        var configurationSerializer = new IniFileConfigurationSerializer(ConfigPath);
        var configuration = new AppConfiguration(configurationSerializer);
        var webHostBuilder = WebHost.CreateDefaultBuilder<Startup>(args);
        webHostBuilder.ConfigureServices(container =>
        {
            container.AddSingleton<IAppConfiguration>(configuration);
        });

        var urls = configuration.GetOrSetDefault("Server:Address", GenerateDefaultUrls).Split(',', StringSplitOptions.TrimEntries);
        if(urls.Any(url => !Uri.TryCreate(url, UriKind.Absolute, out _)))
        {
            Console.WriteLine("Urls not valid.");
            Console.WriteLine("Shutdown application.");
            return;
        }

        webHostBuilder.UseUrls(urls);

        webHostBuilder.Build().Run();
        configuration.Save();
    }

    private static string GenerateDefaultUrls()
    {
        var hostName = Dns.GetHostName();

        return $"http://{hostName}:5000, https://{hostName}:5001";
    }
}