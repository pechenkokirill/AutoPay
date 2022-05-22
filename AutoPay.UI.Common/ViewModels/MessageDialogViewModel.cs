using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

namespace AutoPay.UI.Common.ViewModels;

public class MessageDialogViewModel : DialogViewModel<bool>
{
    public string Message { get; }
    public ICommand CloseCommand { get; }

    public MessageDialogViewModel(string message)
    {
        Message = message;
        CloseCommand = new RelayCommand(() => RequestClose.Invoke(true));
    }
}