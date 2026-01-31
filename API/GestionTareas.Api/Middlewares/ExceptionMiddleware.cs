using System.Net;
using FluentValidation;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using GestionTareas.Domain.Errors;
using GestionTareas.Domain.Exceptions;
using Microsoft.Data.SqlClient;

namespace GestionTareas.Api.Middlewares;

/// <summary>
/// Middleware global de excepciones del API.
/// </summary>
/// <remarks>
/// Este componente es la única fuente responsable de:
/// <list type="bullet">
/// <item><description>Convertir excepciones en códigos HTTP coherentes (400/401/404/409/500).</description></item>
/// <item><description>Definir el formato JSON estándar de error.</description></item>
/// <item><description>Centralizar los mensajes retornados al cliente (evitando textos dispersos en servicios/repositorios/controladores).</description></item>
/// </list>
/// <para>
/// Formato típico de respuesta:
/// <code>
     /*{
    "traceId": "0HMSP0...:00000001",
    "code": "task.status.invalid_transition",
    "message": "No puedes pasar una tarea de Pendiente a Finalizada directamente. Primero cámbiala a En Progreso.",
    "errors": [
    {
        "field": "Title",
        "code": "task.title.required",
        "message": "El título es obligatorio."
    }
    ]
    }*/
/// </code>
/// </para>
/// <para>
/// Nota: Los detalles internos (stack trace, nombres de tablas, etc.) no se exponen al cliente.
/// </para>
/// </remarks>
public class ExceptionMiddleware: IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta el siguiente componente del pipeline y captura cualquier excepción no controlada.
    /// </summary>
    /// <param name="context">Contexto HTTP del request actual.</param>
    /// <param name="next">Delegado del siguiente middleware en la cadena.</param>
    /// <returns>Una tarea asíncrona.</returns>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }
    private async Task HandleAsync(HttpContext context, Exception ex)
    {
        var traceId = context.TraceIdentifier;
        
        _logger.LogError(ex, "Unhandled exception. TraceId={TraceId}", traceId);

        var (statusCode, payload) = MapException(traceId, ex);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }

    private static (HttpStatusCode status, ApiErrorResponse payload) MapException(string traceId, Exception ex)
    {
        if (ex is AppException appEx)
        {
            return (appEx.StatusCode, new ApiErrorResponse
            {
                TraceId = traceId,
                Code = appEx.Code,
                Message = ErrorCatalog.Message(appEx.Code)
            });
        }

        
        if (ex is ValidationException fvEx)
        {
            var fieldErrors = fvEx.Errors
                .Select(f =>
                {
                    var code = string.IsNullOrWhiteSpace(f.ErrorCode) ? ErrorCodes.ValidationFailed : f.ErrorCode;

                    return new ApiFieldError
                    {
                        Field = f.PropertyName,
                        Code = code,
                        Message = ErrorCatalog.Message(code)
                    };
                })
                .ToList();

            return (HttpStatusCode.BadRequest, new ApiErrorResponse
            {
                TraceId = traceId,
                Code = ErrorCodes.ValidationFailed,
                Message = ErrorCatalog.Message(ErrorCodes.ValidationFailed),
                Errors = fieldErrors
            });
        }
        
        if (ex is DbUpdateException dbEx)
        {
            if (dbEx.InnerException?.Message.Contains("IX_Users_Email", StringComparison.OrdinalIgnoreCase) == true ||
                dbEx.InnerException?.Message.Contains("UQ_Users_Email", StringComparison.OrdinalIgnoreCase) == true)
            {
                return (HttpStatusCode.Conflict, new ApiErrorResponse
                {
                    TraceId = traceId,
                    Code = ErrorCodes.UserEmailDuplicate,
                    Message = ErrorCatalog.Message(ErrorCodes.UserEmailDuplicate)
                });
            }
        }
        if (ex is DbUpdateException dbu && dbu.InnerException is SqlException sql)
        {
            if (sql.Number is 2601 or 2627)
            {
                return (HttpStatusCode.Conflict, new ApiErrorResponse
                {
                    TraceId = traceId,
                    Code = ErrorCodes.UserEmailDuplicate,
                    Message = ErrorCatalog.Message(ErrorCodes.UserEmailDuplicate)
                });
            }
        }
        
        return (HttpStatusCode.InternalServerError, new ApiErrorResponse
        {
            TraceId = traceId,
            Code = ErrorCodes.UnexpectedError,
            Message = ErrorCatalog.Message(ErrorCodes.UnexpectedError)
        });
        
    }
    private static string ToCamelCase(string s)
        => string.IsNullOrWhiteSpace(s) ? s : char.ToLowerInvariant(s[0]) + s[1..];
}
