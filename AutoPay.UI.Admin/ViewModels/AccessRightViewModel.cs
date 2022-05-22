using System;
using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.Models;

namespace AutoPay.UI.Admin.ViewModels;

public class AccessRightViewModel
{
    public AccessRightType Right { get; set; }
    public AccessRightViewModel(AccessRightResponse accessRightResponse)
    {
        Right = accessRightResponse.Right;
    }

    public AccessRightViewModel()
    {
        
    }
}