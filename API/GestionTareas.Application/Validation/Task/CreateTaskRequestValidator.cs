using FluentValidation;
using GestionTareas.Application.Dtos;

namespace GestionTareas.Application.Validation.Task;

public sealed class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título de la tarea es obligatorio.")
            .MaximumLength(200).WithMessage("El título no puede superar 200 caracteres.");

        RuleFor(x => x.AssignedUserId)
            .GreaterThan(0).WithMessage("Debes asignar la tarea a un usuario.");

        
        RuleFor(x => x.AdditionalInfoJson)
            .Must(BeValidJsonOrNull)
            .WithMessage("El campo de información adicional debe ser un JSON válido.")
            .When(x => !string.IsNullOrWhiteSpace(x.AdditionalInfoJson));
    }

    private static bool BeValidJsonOrNull(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return true;

        try
        {
            System.Text.Json.JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}