using InnoShop.Users.Application.Abstractions;
using InnoShop.Users.Infrastructure.Auth;
using InnoShop.Users.Infrastructure.Email;
using InnoShop.Users.Infrastructure.Integration;
using InnoShop.Users.Infrastructure.Persistence;
using InnoShop.Users.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.Users.Infrastructure.DI;

public static class UsersInfrastructureExtensions
{
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<UsersDbContext>(opt => opt.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IEmailSender, StubEmailSender>();

        services.AddHttpClient<IProductsInternalClient, ProductsInternalClient>(client =>
        {
            var baseUrl = Environment.GetEnvironmentVariable("ProductsInternal__BaseUrl")
                          ?? "http://localhost:5152";
            client.BaseAddress = new Uri(baseUrl);

            var internalKey = Environment.GetEnvironmentVariable("InternalApiKey") ?? "dev-internal";
            client.DefaultRequestHeaders.Add("X-Internal-Key", internalKey);
        });

        return services;
    }
}
