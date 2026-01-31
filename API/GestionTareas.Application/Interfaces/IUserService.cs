using GestionTareas.Application.Dtos;

namespace GestionTareas.Application.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Crear usuario. 
    /// </summary>
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct);

    /// <summary>
    /// Listar usuarios.
    /// </summary>
    Task<IReadOnlyList<UserResponse>> ListAsync(CancellationToken ct);
}