using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.Models;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using AutoPay.UI.Common.ViewModels;
using Microsoft.Toolkit.Mvvm.Input;
using Refit;

namespace AutoPay.UI.Admin.ViewModels;

public class RegisterUserDialogViewModel : DialogViewModel<UserViewModel>
{
    private readonly IErrorMessageFactory _errorMessageFactory;
    private readonly IDialogService _dialogService;
    private readonly IAutoPayApi _api;
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }

    public bool IsAdmin { get; set; }
    public UserViewModel User { get; set; }

    public RegisterUserDialogViewModel(IErrorMessageFactory errorMessageFactory, IDialogService dialogService, IAutoPayApi api)
    {
        _errorMessageFactory = errorMessageFactory;
        _dialogService = dialogService;
        _api = api;
        User = new UserViewModel
        {
            AccessRights = new() {new AccessRightViewModel {Right = AccessRightType.User}}
        };
        CloseCommand = new RelayCommand(() => RequestClose.Invoke(null));
        SaveCommand = new AsyncRelayCommand(SaveImplementation);
    }

    private async Task SaveImplementation()
    {
        try
        {
            var registrationRequest = new RegistrationRequest
            {
                UserName = User.UserName,
                Password = User.Password,
                FullName = User.FullName,
                IsAdmin = IsAdmin
            };

            var response = await _api.UsersApi.RegisterAsync(registrationRequest);
            User.Id = response.Id;
            User.Password = response.PasswordHash;

            var rights = await _api.AccessRightsApi.GetUserAccessRightAsync(response.Id);
            User.AccessRights = new(rights.Select(x => new AccessRightViewModel(x)));
            RequestClose.Invoke(User);
        }
        catch (ApiException e) when (e.HasContent && e.StatusCode == HttpStatusCode.BadRequest)
        {
            var message = _errorMessageFactory.GetMessage(e, User.Id);
            await _dialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"Ой произошла ошибка: \"{message}\""));
        }
        catch (Exception e)
        {
            await _dialogService.ShowDialogAsync(400, 350,
                new MessageDialogViewModel($"Ой произошла ошибка: \"{e.Message}\""));
        }
    }
}