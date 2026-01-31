using GestionTareas.Application.Dtos;
using GestionTareas.Domain.Entities;

namespace GestionTareas.Application.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem> AddAsync(TaskItem task, CancellationToken ct);
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<TaskItem>> ListAsync(TaskQuery query, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}