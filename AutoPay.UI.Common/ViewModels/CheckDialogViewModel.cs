using System.Drawing.Printing;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.Models;
using AutoPay.UI.Common.Services.Abstraction;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Microsoft.Toolkit.Mvvm.Input;

namespace AutoPay.UI.Common.ViewModels;

public class CheckDialogViewModel : DialogViewModel<bool>
{
    private readonly CheckResponse _check;
    public ICommand CloseCommand { get; set; }
    public ICommand PrintCommand { get; set; }
    public string CheckView { get; }

    public CheckDialogViewModel(ICheckViewBuilder checkViewBuilder, CheckResponse check)
    {
        _check = check;
        CheckView = checkViewBuilder.Build(check);
        CloseCommand = new RelayCommand(() => RequestClose.Invoke(false));
        PrintCommand = new RelayCommand(Print);
    }

    private void Print()
    {
        var dialog = new PrintDialog();
        dialog.PageRangeSelection = PageRangeSelection.AllPages;
        dialog.UserPageRangeEnabled = false;
        if (dialog.ShowDialog() == true)
        {
            var doc = new FlowDocument();
            // For normal A4&/letter pages, the defaults will print in two columns
            doc.PageWidth = 580;
            doc.PageHeight = 400 + (CheckView.Length / 105) * 10;
            doc.ColumnWidth = 580;
            doc.Blocks.Add(new Paragraph(new Run(CheckView)));
            dialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, $"Check: {_check.Id}");
        }
    }
}