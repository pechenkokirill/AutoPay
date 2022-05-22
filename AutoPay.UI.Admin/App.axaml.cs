using System.Net;
using AutoPay.API.Services.Configuration;
using AutoPay.UI.Admin.ViewModels;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services;
using AutoPay.UI.Common.Services.Abstraction;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

namespace AutoPay.UI.Admin
{
    public partial class App : Application
    {
        private const string ConfigPath = "config.ini";
        
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow()
                {
                    DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
                };

                desktop.ShutdownRequested += (sender, args) =>
                {
                    serviceProvider.GetRequiredService<IAppConfiguration>().Save();
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            var serializer = new IniFileConfigurationSerializer(ConfigPath);
            var configuration = new AppConfiguration(serializer);
            serviceCollection.AddSingleton<IAppConfiguration>(configuration);

            var defaultAddress = $"https://{Dns.GetHostName()}:5001";
            var address = configuration.GetOrSetDefault("Server:Address", () => defaultAddress);
            serviceCollection.AddSingleton<IAutoPayApi>(new AutoPayApi(address));
            serviceCollection.AddSingleton<INavigationServiceFactory, NavigationServiceFactory>();
            serviceCollection.AddSingleton<IDialogService, AvaloniaDialogService>();
            serviceCollection.AddSingleton<IServerErrorDeserializer, ServerErrorDeserializer>();
            serviceCollection.AddSingleton<IErrorMessageFactory, ErrorMessageFactory>();
            serviceCollection.AddSingleton<MainWindowViewModel>();
        }
    }
}