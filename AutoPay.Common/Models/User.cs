using AutoPay.Common.Models.Abstraction;

namespace AutoPay.Common.Models;

public sealed class User : IIdentifiable
{
    public static string LocalAdminUserName { get; } = "LocalAdmin";
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public ICollection<AccessRight> AccessRights { get; set; }
}