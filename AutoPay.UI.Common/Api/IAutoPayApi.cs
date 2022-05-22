namespace AutoPay.UI.Common.Api;

public interface IAutoPayApi
{
    ConnectionContext Context { get; }
    IRefitUsers UsersApi { get; }
    IRefitChecks ChecksApi { get; }
    IRefitProducts ProductsApi { get; }
    IRefitAccessRights AccessRightsApi { get; }
    IRefitImages ImagesApi { get; }
}