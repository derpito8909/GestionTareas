using GestionTareas.Domain.Errors;
using GestionTareas.Domain.Exceptions;

namespace GestionTareas.Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private User() { } 

    public User(string name, string email)
    {
        SetName(name);
        SetEmail(email);
    }

    public void UpdateProfile(string name, string email)
    {
        SetName(name);
        SetEmail(email);
    }

    private void SetName(string name)
    {
        name = (name ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleAppException(ErrorCodes.UserNameRequired);

        if (name.Length > 100)
            throw new BusinessRuleAppException(ErrorCodes.UserNameMaxLongitude);

        Name = name;
    }

    private void SetEmail(string email)
    {
        email = (email ?? string.Empty).Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(email))
            throw new BusinessRuleAppException(ErrorCodes.UserEmailRequired);

        if (email.Length > 255)
            throw new BusinessRuleAppException(ErrorCodes.UserEmailMaxLongitude);

        Email = email;
    }
}