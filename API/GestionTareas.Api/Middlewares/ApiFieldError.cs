namespace GestionTareas.Api.Middlewares;

public sealed class ApiFieldError
{
    public string Field { get; init; } = "";
    public string Code { get; init; } = "";
    public string Message { get; init; } = "";
}