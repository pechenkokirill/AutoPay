using AutoPay.Common.DTOs.Responses;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoPay.UI.Client.ViewModels;

public class UserViewModel : ObservableObject
{
    public string Name { get; set; }
    
    public UserViewModel(UserResponse user)
    {
        Name = user.FullName;
    }
}