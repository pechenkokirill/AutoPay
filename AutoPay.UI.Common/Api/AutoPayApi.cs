using System.Net.Http;
using Refit;

namespace AutoPay.UI.Common.Api;

public class AutoPayApi : IAutoPayApi
{
    public ConnectionContext Context { get; }
    public IRefitUsers UsersApi { get; }
    public IRefitChecks ChecksApi { get; }
    public IRefitProducts ProductsApi { get; }
    public IRefitAccessRights AccessRightsApi { get; }
    public IRefitImages ImagesApi { get; }

    public AutoPayApi(string hostUrl)
    {
        Context = new ConnectionContext();
        var handler = new JwtHttpClientHandler(Context);
        handler.ServerCertificateCustomValidationCallback += (message, certificate2, arg3, arg4) => true; 
        var client = new HttpClient(handler);
        client.BaseAddress = new Uri(hostUrl);

        UsersApi = RestService.For<IRefitUsers>(client);
        ChecksApi = RestService.For<IRefitChecks>(client);
        ProductsApi = RestService.For<IRefitProducts>(client);
        AccessRightsApi = RestService.For<IRefitAccessRights>(client);
        ImagesApi = RestService.For<IRefitImages>(client);
    }
}