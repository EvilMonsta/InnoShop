using FluentValidation;
using InnoShop.Users.Application.Users.Commands;
using System.Text.RegularExpressions;

namespace InnoShop.Users.Application.Validation;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    private static readonly Regex EmailRx =
        new(@"^[^@\s]+@[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,24}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public RegisterUserValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email)
            .NotEmpty()
            .Must(v => v is not null && EmailRx.IsMatch(v))
            .WithMessage("Email должен быть вида user@domain.tld (например user@mail.ru, user@gmail.com, user@hb.by)");
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}