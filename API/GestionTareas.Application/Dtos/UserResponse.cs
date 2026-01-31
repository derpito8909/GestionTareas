namespace GestionTareas.Application.Dtos;

public sealed record UserResponse(int Id, string Name, string Email, DateTime CreatedAt);