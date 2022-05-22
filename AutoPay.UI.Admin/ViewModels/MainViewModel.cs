using System.Threading.Tasks;
using System.Windows.Input;
using AutoPay.Common.DTOs.Responses;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace AutoPay.UI.Admin.ViewModels;

public class MainViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly IAutoPayApi _api;
    private readonly IErrorMessageFactory _errorMessageFactory;
    public string CurrentUserName { get; set; }
    public string CurrentFullName { get; set; }
    public INavigationService NavigationService { get; }

    public ICommand ImagesEditPageCommand { get; set; }
    public ICommand ProductsEditPageCommand { get; set; }
    public ICommand UsersEditPageCommand { get; set; }
    public ICommand ChecksEditPageCommand { get; set; }

    public MainViewModel(UserResponse response,
        IDialogService dialogService,
        IAutoPayApi api,
        INavigationServiceFactory navigationServiceFactory,
        IErrorMessageFactory errorMessageFactory)
    {
        _dialogService = dialogService;
        _api = api;
        _errorMessageFactory = errorMessageFactory;
        NavigationService = navigationServiceFactory.Create();
        CurrentUserName = response.UserName;
        CurrentFullName = response.FullName;
        ImagesEditPageCommand = new AsyncRelayCommand(NavigateToImageEdit);
        ProductsEditPageCommand = new AsyncRelayCommand(NavigateProductsEdit);
        UsersEditPageCommand = new AsyncRelayCommand(NavigateUsersEdit);
        ChecksEditPageCommand = new AsyncRelayCommand(NavigateChecksEdit);
    }

    private async Task NavigateChecksEdit()
    {
        var viewModel = new ChecksEditViewModel(_dialogService, _api);
        NavigationService.Navigate(viewModel);
        await viewModel.LoadAsync();
    }

    private async Task NavigateUsersEdit()
    {
        var viewModel = new UsersEditViewModel(_errorMessageFactory, _dialogService, _api);
        NavigationService.Navigate(viewModel);
        await viewModel.LoadAsync();
    }

    private async Task NavigateProductsEdit()
    {
        var viewModel = new ProductsEditViewModel(_errorMessageFactory, _dialogService, _api);
        NavigationService.Navigate(viewModel);
        await viewModel.LoadAsync();
    }

    private async Task NavigateToImageEdit()
    {
        var startViewModel = new ImagesEditViewModel(_errorMessageFactory, _dialogService, _api);
        NavigationService.Navigate(startViewModel);
        await startViewModel.LoadAsync();
    }

    public async Task InitStartPageAsync()
    {
        await NavigateToImageEdit();
    }
}