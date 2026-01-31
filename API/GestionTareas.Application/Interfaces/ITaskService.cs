using GestionTareas.Application.Dtos;

namespace GestionTareas.Application.Interfaces;

public interface ITaskService
{
    /// <summary>
    /// Crear tarea (queda asignada a un usuario).
    /// </summary>
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken ct);

    /// <summary>
    /// Listar tareas con filtro opcional por usuario/estado y orden por CreatedAt.
    /// </summary>
    Task<IReadOnlyList<TaskResponse>> ListAsync(TaskQuery query, CancellationToken ct);

    /// <summary>
    /// Asignar una tarea existente a un usuario.
    /// </summary>
    Task<TaskResponse> AssignAsync(int taskId, AssignTaskRequest request, CancellationToken ct);

    /// <summary>
    /// Cambiar estado de una tarea.
    /// </summary>
    Task<TaskResponse> ChangeStatusAsync(int taskId, ChangeTaskStatusRequest request, CancellationToken ct);
}