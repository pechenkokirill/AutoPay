using AutoPay.Common.Models.Abstraction;

namespace AutoPay.Common.Models;

public sealed class AccessRight : IIdentifiable
{
    public Guid Id { get; set; }
    public AccessRightType Type { get; set; }

    public ICollection<User> Users { get; set; }
}