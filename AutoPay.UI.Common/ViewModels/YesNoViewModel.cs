using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

namespace AutoPay.UI.Common.ViewModels;

public class YesNoViewModel : DialogViewModel<bool>
{
    public ICommand YesCommand { get; }
    public ICommand NoCommand { get; }
    public string Message { get; }
    public YesNoViewModel(string message)
    {
        Message = message;
        YesCommand = new RelayCommand(() => RequestClose.Invoke(true));
        NoCommand = new RelayCommand(() => RequestClose.Invoke(false));
    }
}