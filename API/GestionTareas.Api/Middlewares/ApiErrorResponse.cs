namespace GestionTareas.Api.Middlewares;

public sealed class ApiErrorResponse
{
    public string TraceId { get; init; } = "";
    public string Code { get; init; } = "";
    public string Message { get; init; } = "";
    public List<ApiFieldError>? Errors { get; init; }
}