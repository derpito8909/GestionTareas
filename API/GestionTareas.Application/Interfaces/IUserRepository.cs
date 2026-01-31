using GestionTareas.Domain.Entities;

namespace GestionTareas.Application.Interfaces;

public interface IUserRepository
{
    Task<User> AddAsync(User user, CancellationToken ct);
    Task<User?> GetByIdAsync(int userId, CancellationToken ct);
    Task<IReadOnlyList<User>> ListAsync(CancellationToken ct);
    Task<bool> ExistsAsync(int userId, CancellationToken ct);
    Task<bool> EmailExistsAsync(string emailNormalized, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}