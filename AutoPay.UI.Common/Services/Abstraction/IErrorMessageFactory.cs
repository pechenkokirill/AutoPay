using Refit;

namespace AutoPay.UI.Common.Services.Abstraction;

public interface IErrorMessageFactory
{
    string GetMessage(ApiException exception, object parameter);
}