using FluentValidation;
using GestionTareas.Application.Dtos;
using GestionTareas.Application.Interfaces;
using GestionTareas.Domain.Entities;

namespace GestionTareas.Application.Services;

public sealed class UsersService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IValidator<CreateUserRequest> _createValidator;

    public UsersService(IUserRepository users, IValidator<CreateUserRequest> createValidator)
    {
        _users = users;
        _createValidator = createValidator;
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        
        var user = new User(request.Name, request.Email);

        user = await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        return new UserResponse(user.Id, user.Name, user.Email, user.CreatedAt);
    }

    public async Task<IReadOnlyList<UserResponse>> ListAsync(CancellationToken ct)
    {
        var users = await _users.ListAsync(ct);

        return users
            .Select(u => new UserResponse(u.Id, u.Name, u.Email, u.CreatedAt))
            .ToList();
    }
}