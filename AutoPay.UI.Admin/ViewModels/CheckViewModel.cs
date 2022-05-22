using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.Models;
using AutoPay.UI.Common.Api;
using AutoPay.UI.Common.ViewModels;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoPay.UI.Admin.ViewModels;

public class CheckViewModel : ObservableObject
{
    public Guid Id { get; set; }
    public string Issuer { get; set; }
    public DateTime IssueDate { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal ActualPaymentAmount { get; set; }
    
    public IEnumerable<CheckLineProductViewModel> Lines { get; set; }

    public CheckViewModel(CheckResponse checkResponse, IAutoPayApi api)
    {
        Id = checkResponse.Id;
        Issuer = checkResponse.Issuer.UserName;
        IssueDate = checkResponse.IssueDate;
        PaymentAmount = checkResponse.PaymentAmount;
        ActualPaymentAmount = checkResponse.ActualPaymentAmount;

        Lines = checkResponse.Lines.Select(x => new CheckLineProductViewModel(new ProductViewModel(api, x.Product), null)
        {
            Count = x.Count
        }).ToList();
    }

    public async Task InitializeAsync()
    {
        foreach (var checkLineProductViewModel in Lines)
        {
            await checkLineProductViewModel.InitializeProductAsync();
        }
    }
}