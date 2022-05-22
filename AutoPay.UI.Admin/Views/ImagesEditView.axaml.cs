using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AutoPay.UI.Admin.Views;

public partial class ImagesEditView : UserControl
{
    public ImagesEditView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}