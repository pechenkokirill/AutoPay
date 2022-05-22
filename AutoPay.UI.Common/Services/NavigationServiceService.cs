using AutoPay.UI.Common.Services.Abstraction;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoPay.UI.Common.Services;

public class NavigationServiceService : ObservableObject, INavigationService
{
    public ObservableObject Page { get; private set; }
    public void Navigate(ObservableObject viewModel)
    {
        Page = viewModel;
        OnPropertyChanged(nameof(Page));
    }
}