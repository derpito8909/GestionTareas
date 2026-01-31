using FluentValidation;
using GestionTareas.Application.Dtos;

namespace GestionTareas.Application.Validation.Task;

public sealed class ChangeTaskStatusRequestValidator : AbstractValidator<ChangeTaskStatusRequest>
{
    public ChangeTaskStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(string.Empty);
    }
}