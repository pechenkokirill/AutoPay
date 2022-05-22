using System.Linq;
using AutoPay.API.Services.Configuration;
using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.Models;
using AutoPay.UI.Client.ViewModels;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using AutoPay.UI.Common.ViewModels;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoPay.UI.Admin.ViewModels;

public class MainWindowViewModel : ObservableObject
{
    private readonly IAutoPayApi _api;
    private readonly IDialogService _dialogService;
    private readonly INavigationServiceFactory _navigationServiceFactory;
    private readonly IErrorMessageFactory _errorMessageFactory;
    public string CurrentUserName { get; set; }
    public LoginViewModel LoginViewModel { get; }
    public INavigationService NavigationService { get; }

    public MainWindowViewModel(IAutoPayApi api, IDialogService dialogService, INavigationServiceFactory navigationServiceFactory,
        IAppConfiguration appConfiguration, IErrorMessageFactory errorMessageFactory)
    {
        _api = api;
        _dialogService = dialogService;
        _navigationServiceFactory = navigationServiceFactory;
        _errorMessageFactory = errorMessageFactory;
        NavigationService = navigationServiceFactory.Create();
        LoginViewModel = new LoginViewModel(api, appConfiguration);
        LoginViewModel.OnLogin += OnLogin;
        NavigationService.Navigate(LoginViewModel);
    }

    private async void OnLogin(UserResponse response)
    {
        var rights = await _api.AccessRightsApi.GetUserAccessRightAsync(response.Id);

        if (rights.All(x => x.Right != AccessRightType.Admin))
        {
            await _dialogService.ShowDialogAsync(500,300,new MessageDialogViewModel($"У пользователя '{response.UserName}' нет прав доступа!"));
            return;
        }
        
        var mainViewModel = new MainViewModel(response, _dialogService, _api, _navigationServiceFactory, _errorMessageFactory);
        NavigationService.Navigate(mainViewModel);
        await mainViewModel.InitStartPageAsync();
    }
}