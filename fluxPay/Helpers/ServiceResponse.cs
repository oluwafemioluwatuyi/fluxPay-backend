using System;
using fluxPay.Constants;

namespace fluxPay.Helpers;

public class ServiceResponse<T>
{
    public ResponseStatus Status { get; set; }
    public string StatusCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public T? Result { get; set; } = default(T);
    public ServiceResponse(ResponseStatus status, AppStatusCodes statusCode, string message, T? result)
    {
        Status = status;
        Message = message;
        Result = result;
        StatusCode = ((int)statusCode).ToString("D4");
    }

}
