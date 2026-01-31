using System.Net;

namespace GestionTareas.Domain.Exceptions;

/// <summary>
/// Excepción que indica que un recurso solicitado no existe.
/// Se usa para que el middleware devuelva HTTP 404 (Not Found) con un mensaje claro.
/// </summary>
public class NotFoundException :AppException
{
    public NotFoundException(string code) : base(code, HttpStatusCode.NotFound) { }
}