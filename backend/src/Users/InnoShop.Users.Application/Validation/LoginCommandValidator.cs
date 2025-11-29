using FluentValidation;
using InnoShop.Users.Application.Users.Commands;


namespace InnoShop.Users.Application.Validation;


public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}