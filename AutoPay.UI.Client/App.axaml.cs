using System.Linq;
using System.Net;
using AutoPay.API.Services.Configuration;
using AutoPay.UI.Client.ViewModels;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services;
using AutoPay.UI.Common.Services.Abstraction;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;

namespace AutoPay.UI.Client
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
            serviceCollection.AddSingleton<ICheckViewBuilder, CheckViewBuilder>();
            serviceCollection.AddSingleton<MainWindowViewModel>();
        }
    }
}