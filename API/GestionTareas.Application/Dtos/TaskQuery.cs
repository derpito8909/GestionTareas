using TaskStatus = GestionTareas.Domain.Enums.TaskStatus;

namespace GestionTareas.Application.Dtos;

/// <summary>
/// Parámetros opcionales para listar/filtrar tareas.
/// </summary>
public sealed record TaskQuery(
    int? UserId = null,
    TaskStatus? Status = null,
    bool OrderByCreatedAtDesc = true,
    
    string? Priority = null,         
    string? Tag = null,              
    DateTime? DueDateFrom = null,     
    DateTime? DueDateTo = null 
)
{
    public bool HasJsonFilters =>
        !string.IsNullOrWhiteSpace(Priority)
        || !string.IsNullOrWhiteSpace(Tag)
        || DueDateFrom.HasValue
        || DueDateTo.HasValue;
}