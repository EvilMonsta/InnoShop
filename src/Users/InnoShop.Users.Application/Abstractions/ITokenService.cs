using InnoShop.Users.Domain.Users;


namespace InnoShop.Users.Application.Abstractions;


public interface ITokenService
{
    string GenerateAccessToken(User user);
}