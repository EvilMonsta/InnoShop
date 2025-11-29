using FluentValidation;
using InnoShop.Users.Contracts.Requests;
using System.Text.RegularExpressions;

namespace InnoShop.Users.Api.Validation;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    private static readonly Regex EmailRx =
        new(@"^[^@\s]+@[A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*\.[A-Za-z]{2,24}$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .Matches(EmailRx)
            .WithMessage("Email должен быть вида user@domain.tld (например user@mail.ru, user@gmail.com, user@hb.by)");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}
