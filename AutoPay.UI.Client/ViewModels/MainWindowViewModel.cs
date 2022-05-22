using AutoPay.API.Services.Configuration;
using AutoPay.Common.DTOs.Responses;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoPay.UI.Client.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    private readonly IAutoPayApi _api;
    private readonly ICheckViewBuilder _checkViewBuilder;
    private readonly IDialogService _dialogService;
    public LoginViewModel LoginViewModel { get; }
    public INavigationService NavigationService { get; }

    public MainWindowViewModel(IAutoPayApi api, ICheckViewBuilder checkViewBuilder, IDialogService dialogService,INavigationServiceFactory navigationServiceFactory, IAppConfiguration appConfiguration)
    {
        _api = api;
        _checkViewBuilder = checkViewBuilder;
        _dialogService = dialogService;
        NavigationService = navigationServiceFactory.Create();
        LoginViewModel = new LoginViewModel(api, appConfiguration);
        LoginViewModel.OnLogin += OnLogin;
        NavigationService.Navigate(LoginViewModel);
    }

    private async void OnLogin(UserResponse response)
    {
        var mainViewModel = new MainViewModel(_api, _checkViewBuilder, _dialogService, response);
        NavigationService.Navigate(mainViewModel);
        await mainViewModel.LoadAsync();
    }
}