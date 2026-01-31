using GestionTareas.Application.Interfaces;
using GestionTareas.Domain.Entities;
using GestionTareas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GestionTareas.Infrastructure.Repositories;

public sealed class UsersRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UsersRepository(AppDbContext db) => _db = db;

    public async Task<User> AddAsync(User user, CancellationToken ct)
    {
        await _db.Users.AddAsync(user, ct);
        return user;
    }

    public Task<User?> GetByIdAsync(int userId, CancellationToken ct)
        => _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, ct);

    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken ct)
        => await _db.Users.AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public Task<bool> ExistsAsync(int userId, CancellationToken ct)
        => _db.Users.AsNoTracking().AnyAsync(x => x.Id == userId, ct);

    public Task<bool> EmailExistsAsync(string emailNormalized, CancellationToken ct)
        => _db.Users.AsNoTracking().AnyAsync(x => x.Email == emailNormalized, ct);

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}