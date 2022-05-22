using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.Models;

namespace AutoPay.UI.Admin.ViewModels;

public class UserViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public ObservableCollection<AccessRightViewModel> AccessRights { get; set; }

    public UserViewModel(UserResponse response, IEnumerable<AccessRightViewModel> accessRightViewModels)
    {
        Id = response.Id;
        UserName = response.UserName;
        FullName = response.FullName;
        Password = response.PasswordHash;
        AccessRights = new ObservableCollection<AccessRightViewModel>(accessRightViewModels);
    }

    public UserViewModel()
    {
        
    }
}