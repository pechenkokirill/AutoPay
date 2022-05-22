using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoPay.API.Services.Configuration;
using AutoPay.Common.DTOs.Requests;
using AutoPay.Common.DTOs.Responses;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.Services.Abstraction;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Refit;

namespace AutoPay.UI.Client.ViewModels;

public class LoginViewModel : ObservableObject
{
    private bool _isLoading, _savePassword;
    private string _error;
    private readonly IAutoPayApi _api;
    private readonly IAppConfiguration _configuration;

    public event Action<UserResponse> OnLogin; 
    public string UserName { get; set; }
    public string PassWord { get; set; }
    public string Error
    {
        get => _error;
        set => SetProperty(ref _error, value);
    }

    public bool SavePassword
    {
        get => _savePassword;
        set => SetProperty(ref _savePassword, value);
    }
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    public ICommand LoginCommand { get; set; }
    public LoginViewModel(IAutoPayApi api, IAppConfiguration configuration)
    {
        _api = api;
        _configuration = configuration;
        UserName = _configuration.GetOrSetDefault("Account:UserName", () => "User");
        PassWord = _configuration.GetValue("Account:PassWord") ?? string.Empty;
        LoginCommand = new AsyncRelayCommand(Login);
    }

    private async Task Login(CancellationToken cancellationToken)
    {
        IsLoading = true;
        try
        {
            var authenticationResponse = await _api.UsersApi.AuthenticateAsync(new AuthenticationCredentialsRequest
            {
                UserName = UserName,
                Password = PassWord
            });
            
            _configuration.SetValue("Account:UserName", UserName);

            if (SavePassword)
            {
                _configuration.SetValue("Account:PassWord", PassWord);
            }
            
            _configuration.Save();
            _api.Context.Token = authenticationResponse.Token;
            OnLogin?.Invoke(authenticationResponse.User);
        }
        catch (ApiException e)
        {
            switch (e.StatusCode)
            {
                case HttpStatusCode.InternalServerError:
                    Error = "Ошибка сервера!";
                    break;
                case HttpStatusCode.BadRequest:
                    Error = "Неверное имя пользователя или пароль!";
                    break;
                default:
                    Error = "Неизвестная ошибка!";
                    break;
            }
        }
        catch (Exception e)
        {
            Error = e.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}