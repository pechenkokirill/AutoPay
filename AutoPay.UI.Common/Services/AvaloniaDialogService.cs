using AutoPay.UI.Common.Services.Abstraction;
using AutoPay.UI.Common.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using JetBrains.Annotations;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoPay.UI.Common.Services;

public class AvaloniaDialogService : IDialogService
{
    public async Task<TResult> ShowDialogAsync<TResult>(double width, double height, DialogViewModel<TResult> vm)
    {
        var window = new Window
        {
            DataContext = vm,
            Content = vm,
            Width = width,
            Height = height,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            SystemDecorations = SystemDecorations.BorderOnly,
            CornerRadius = new CornerRadius(5),
            TransparencyLevelHint = WindowTransparencyLevel.Transparent,
            Background = Brushes.Transparent
        };

        vm.RequestClose = result => window.Close(result);

        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime lifetime)
        {
            throw new NotSupportedException(nameof(Application.Current.ApplicationLifetime));
        }

        return await window.ShowDialog<TResult>(lifetime.MainWindow);
    }

    public async Task<string> OpenFileDialogAsync(List<FileDialogFilter> filters)
    {
        var fileDialog = new OpenFileDialog
        {
            AllowMultiple = false,
            Filters = filters
        };
        
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime lifetime)
        {
            throw new NotSupportedException(nameof(Application.Current.ApplicationLifetime));
        }

        var result = await fileDialog.ShowAsync(lifetime.MainWindow);
        
        return result?[0];
    }
}