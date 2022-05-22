using System.Text.Json.Serialization;

namespace AutoPay.Common.DTOs.Responses.Errors;

public class ErrorResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]  
    public InternalErrorCode ErrorCode { get; }

    public ErrorResponse(InternalErrorCode errorCode)
    {
        ErrorCode = errorCode;
    }
}