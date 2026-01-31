using FluentValidation;
using GestionTareas.Application.Dtos;
using GestionTareas.Application.Interfaces;
using GestionTareas.Domain.Errors;

namespace GestionTareas.Application.Validation.Users;

public class CreateUserRequestValidator: AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator(IUserRepository users)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithErrorCode(ErrorCodes.UserNameRequired)
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(ErrorCodes.UserEmailRequired)
            .EmailAddress().WithErrorCode(ErrorCodes.UserEmailInvalid)
            .MaximumLength(255)
            .MustAsync(async (email, ct) => !await users.EmailExistsAsync(email.Trim().ToLowerInvariant(), ct))
            .WithErrorCode(ErrorCodes.UserEmailDuplicate);
    }
}