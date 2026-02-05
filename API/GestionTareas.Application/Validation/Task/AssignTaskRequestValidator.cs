using FluentValidation;
using GestionTareas.Application.Dtos;
using GestionTareas.Domain.Errors;
namespace GestionTareas.Application.Validation.Task;

public sealed class AssignTaskRequestValidator : AbstractValidator<AssignTaskRequest>
{
    public AssignTaskRequestValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithErrorCode(ErrorCodes.TaskAssignedUserRequired);
    }
}