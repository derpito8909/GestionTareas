namespace GestionTareas.Application.Dtos;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    int AssignedUserId,
    string? AdditionalInfoJson 
);