using System.Text.Json;
using AutoPay.Common.DTOs.Responses.Errors;
using AutoPay.UI.Common.Services.Abstraction;
using Refit;

namespace AutoPay.UI.Common.Services;

public class ServerErrorDeserializer : IServerErrorDeserializer
{
    private readonly JsonSerializerOptions _options;

    public ServerErrorDeserializer()
    {
        _options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }
    
    public ErrorResponse Deserialize(ApiException exception)
    {
        if (exception.Content is null)
        {
            return default;
        }
        
        return JsonSerializer.Deserialize<ErrorResponse>(exception.Content, _options);
    }
}