using AutoPay.Common.DTOs.Responses;
using AutoPay.Common.Models;

namespace AutoPay.UI.Common.Services.Abstraction;

public interface ICheckViewBuilder
{
    string Build(CheckResponse check);
}