using AutoPay.Common.DTOs.Responses.Errors;
using AutoPay.UI.Common.Services.Abstraction;
using Refit;

namespace AutoPay.UI.Common.Services;

public class ErrorMessageFactory : IErrorMessageFactory
{
    private readonly IServerErrorDeserializer _serverErrorDeserializer;

    public ErrorMessageFactory(IServerErrorDeserializer serverErrorDeserializer)
    {
        _serverErrorDeserializer = serverErrorDeserializer;
    }
    
    public string GetMessage(ApiException exception, object parameter)
    {
        var errorResponse = _serverErrorDeserializer.Deserialize(exception);

        switch (errorResponse.ErrorCode)
        {
            case InternalErrorCode.AlreadyExist:
                return $"Объект с таким id '{parameter}' уже существует!";
            case InternalErrorCode.NotFound:
                return $"Объект с таким id '{parameter}' не найден!";
            case InternalErrorCode.ImageNotFound:
                return $"Картинки с id '{parameter}' не существует сервере!";
            case InternalErrorCode.ImageBadFormat:
                return "Картинка должно быть в формате png!";
            case InternalErrorCode.ProductAlreadyExist:
                return $"Продукт с таким номером '{parameter}' уже существует!";
            case InternalErrorCode.DeletingLocalAdmin:
                return "Нельзя удалить локального администратора!";
            case InternalErrorCode.EditingLocalAdmin:
                return "Нельзя изменить локального администратора!";
            default:
                return null;
        }
    }
}