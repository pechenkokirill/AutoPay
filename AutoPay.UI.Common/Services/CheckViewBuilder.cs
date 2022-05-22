using System.Globalization;
using System.Text;
using AutoPay.API.Services.Configuration;
using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.Models;
using AutoPay.UI.Common.Converters;
using AutoPay.UI.Common.Services.Abstraction;

namespace AutoPay.UI.Common.Services;

public class CheckViewBuilder : ICheckViewBuilder
{
    private readonly IAppConfiguration _appConfiguration;

    public CheckViewBuilder(IAppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
    }
    
    public string Build(CheckResponse check)
    {
        StringBuilder sb = new StringBuilder();

        var companyName = _appConfiguration.GetOrSetDefault("Settings:CompanyName", () => "MyCompany");
        var spacesCount = (110 - companyName.Length) / 2 + 2;
        var companyNameDecorated = companyName.PadLeft(spacesCount).PadRight(spacesCount);
        
        sb.AppendLine(new string('#', companyNameDecorated.Length ));
        sb.AppendLine(companyNameDecorated);
        sb.AppendLine(new string('#', companyNameDecorated.Length));
        sb.AppendLine("Идентификационный номер: " + check.Id);
        sb.AppendLine("Кассир: " + check.Issuer.FullName);
        sb.AppendLine("Дата выдачи: " + check.IssueDate);
        sb.AppendLine();
        
        foreach (var line in check.Lines)
        {
            var name = line.Product.ProductId + " " + line.Product.Name;
            var unit = UnitConverter.Instance.Convert(line.Product.Unit, typeof(string), null, CultureInfo.CurrentCulture) as string;
            var info = $"{line.Product.Cost} x {line.Count} {unit} ({line.Product.Cost * (decimal) line.Count}) бел руб.";

            var lineSpacesCount = 105 - (name.Length + info.Length);
            
            sb.AppendLine(name.PadRight(lineSpacesCount) + info);
        }
        
        sb.AppendLine();
        sb.AppendLine("Итого: " + check.PaymentAmount);
        sb.AppendLine("Оплачено: " + check.ActualPaymentAmount);
        sb.AppendLine("Сдача: " + (check.ActualPaymentAmount - check.PaymentAmount));
        sb.AppendLine(new string('#', companyNameDecorated.Length));
        return sb.ToString();
    }
}