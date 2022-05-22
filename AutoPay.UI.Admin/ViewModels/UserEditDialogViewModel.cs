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

public class UserEditDialogViewModel : DialogViewModel<bool>
{
    private readonly UserViewModel _editableUser;
    private readonly IErrorMessageFactory _errorMessageFactory;
    private readonly IDialogService _dialogService;
    private readonly IAutoPayApi _api;
    public ICommand CloseCommand { get; }
    public ICommand SaveCommand { get; }

    public bool IsAdmin { get; set; }
    public UserViewModel User { get; set; }

    public UserEditDialogViewModel(UserViewModel editableUser, IErrorMessageFactory errorMessageFactory, IDialogService dialogService,
        IAutoPayApi api)
    {
        _editableUser = editableUser;
        _errorMessageFactory = errorMessageFactory;
        _dialogService = dialogService;
        _api = api;
        User = new UserViewModel
        {
            Id = editableUser.Id,
            Password = string.Empty,
            FullName = editableUser.FullName,
            UserName = editableUser.UserName,
            AccessRights = editableUser.AccessRights
        };
        IsAdmin = editableUser.AccessRights.Any(x => x.Right == AccessRightType.Admin);
        
        CloseCommand = new RelayCommand(() => RequestClose.Invoke(false));
        SaveCommand = new AsyncRelayCommand(SaveImplementation);
    }

    private async Task SaveImplementation()
    {
        try
        {
            var registrationRequest = new UserInformationRequest()
            {
                UserName = User.UserName,
                Password = User.Password,
                FullName = User.FullName,
                IsAdmin = IsAdmin
            };

            var response = await _api.UsersApi.EditUsersAsync(User.Id, registrationRequest);
            _editableUser.UserName = response.UserName;
            _editableUser.FullName = response.FullName;
            _editableUser.Password = response.PasswordHash;

            var rights = await _api.AccessRightsApi.GetUserAccessRightAsync(response.Id);
            
            _editableUser.AccessRights.Clear();
            foreach (var right in rights)
            {
                _editableUser.AccessRights.Add(new AccessRightViewModel(right));
            }
            
            RequestClose.Invoke(true);
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