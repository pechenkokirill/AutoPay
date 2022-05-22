namespace AutoPay.Common.DTOs.Responses.Errors;

public enum InternalErrorCode
{
    AlreadyExist,
    NotFound,
    ImageNotFound,
    ImageBadFormat,
    ProductAlreadyExist,
    DeletingLocalAdmin,
    EditingLocalAdmin
}