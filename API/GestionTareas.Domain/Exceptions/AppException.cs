using System.Net;

namespace GestionTareas.Domain.Exceptions;

public class AppException : Exception
{
    public string Code { get; }
    public HttpStatusCode StatusCode { get; }

    public AppException(string code, HttpStatusCode statusCode)
    {
        Code = code;
        StatusCode = statusCode;
    }
}