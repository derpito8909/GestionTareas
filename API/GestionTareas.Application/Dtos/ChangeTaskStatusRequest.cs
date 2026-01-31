using TaskStatus = GestionTareas.Domain.Enums.TaskStatus;

namespace GestionTareas.Application.Dtos;

public sealed record ChangeTaskStatusRequest(TaskStatus Status);