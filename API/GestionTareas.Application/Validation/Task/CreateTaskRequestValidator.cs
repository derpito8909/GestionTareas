using System.Text.Json;
using FluentValidation;
using GestionTareas.Application.Dtos;
using GestionTareas.Domain.Errors;

namespace GestionTareas.Application.Validation.Task;

public sealed class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithErrorCode(ErrorCodes.TaskTitleRequired).WithMessage(string.Empty)
            .MaximumLength(200).WithMessage(string.Empty);

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage(string.Empty);

        RuleFor(x => x.AssignedUserId)
            .GreaterThan(0).WithErrorCode(ErrorCodes.TaskAssignedUserRequired).WithMessage(string.Empty);

        RuleFor(x => x.AdditionalInfoJson)
            .Must(BeValidJson)
            .WithErrorCode(ErrorCodes.TaskAdditionalInfoInvalidJson).WithMessage(string.Empty)
            .When(x => !string.IsNullOrWhiteSpace(x.AdditionalInfoJson));
    }

    private static bool BeValidJson(string? json)
    {
        try { JsonDocument.Parse(json!); return true; }
        catch { return false; }
    }
}