using FluentValidation;
using GestionTareas.Application.Dtos;
using GestionTareas.Domain.Errors;

namespace GestionTareas.Application.Validation.Task;

public sealed class ChangeTaskStatusRequestValidator : AbstractValidator<ChangeTaskStatusRequest>
{
    public ChangeTaskStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(ErrorCodes.TaskInvalidTransition);
    }
}