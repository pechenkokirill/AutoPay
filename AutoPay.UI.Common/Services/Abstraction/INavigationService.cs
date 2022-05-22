using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoPay.UI.Common.Services.Abstraction;

public interface INavigationService
{
    ObservableObject Page { get; }
    
    void Navigate(ObservableObject viewModel);
}