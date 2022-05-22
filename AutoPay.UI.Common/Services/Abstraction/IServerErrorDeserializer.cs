using AutoPay.Common.DTOs.Responses.Errors;
using Refit;

namespace AutoPay.UI.Common.Services.Abstraction;

public interface IServerErrorDeserializer
{
    ErrorResponse Deserialize(ApiException exception);
}