using AutoPay.UI.Common.ViewModels;
using Avalonia.Controls;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoPay.UI.Common.Services.Abstraction;

public interface IDialogService
{
    Task<TResult> ShowDialogAsync<TResult>(double width, double height, DialogViewModel<TResult> vm);

    Task<string> OpenFileDialogAsync(List<FileDialogFilter> filters);
}