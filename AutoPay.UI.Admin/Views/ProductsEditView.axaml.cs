using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AutoPay.UI.Admin.Views;

public partial class ProductsEditView : UserControl
{
    public ProductsEditView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}