using TaskStatus = GestionTareas.Domain.Enums.TaskStatus;
namespace GestionTareas.Application.Dtos;

public sealed record TaskResponse(
    int Id,
    string Title,
    string? Description,
    TaskStatus Status,
    DateTime CreatedAt,
    int AssignedUserId,
    string AssignedUserName,
    string? AdditionalInfoJson
);