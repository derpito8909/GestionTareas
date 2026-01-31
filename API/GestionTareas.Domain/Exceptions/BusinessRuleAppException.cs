using System.Net;

namespace GestionTareas.Domain.Exceptions;

public class BusinessRuleAppException: AppException
{
public BusinessRuleAppException(string code) : base(code, HttpStatusCode.BadRequest) { }
}