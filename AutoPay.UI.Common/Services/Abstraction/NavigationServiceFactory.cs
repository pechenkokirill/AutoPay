namespace AutoPay.UI.Common.Services.Abstraction;

public class NavigationServiceFactory : INavigationServiceFactory
{
    public INavigationService Create()
    {
        return new NavigationServiceService();
    }
}