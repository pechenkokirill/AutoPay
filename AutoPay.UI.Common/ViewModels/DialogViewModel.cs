using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace AutoPay.UI.Common.ViewModels;

public class DialogViewModel<TResult> : ObservableObject
{
    public Action<TResult> RequestClose { get; set; }

}