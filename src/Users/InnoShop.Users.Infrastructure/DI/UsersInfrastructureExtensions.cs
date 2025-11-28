using InnoShop.Users.Application.Abstractions;
using InnoShop.Users.Infrastructure.Auth;
using InnoShop.Users.Infrastructure.Email;
using InnoShop.Users.Infrastructure.Persistence;
using InnoShop.Users.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using InnoShop.Users.Infrastructure.Internal;

namespace InnoShop.Users.Infrastructure.DI;

public static class UsersInfrastructureExtensions
{
    public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<UsersDbContext>(opt => opt.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IEmailTokenRepository, EmailTokenRepository>();
        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddSingleton<IEmailSender>(sp =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>();
            var host = cfg["Smtp:Host"];
            return string.IsNullOrWhiteSpace(host)
                ? new StubEmailSender(sp.GetRequiredService<ILogger<StubEmailSender>>())
                : new SmtpEmailSender(cfg, sp.GetRequiredService<ILogger<SmtpEmailSender>>());
        });

        services.AddHttpClient<IProductsInternalClient, ProductsInternalClient>((sp, http) =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>();
            var baseUrl = cfg["ProductsInternal:BaseUrl"]
                          ?? throw new InvalidOperationException("ProductsInternal:BaseUrl is not set");
            var key = cfg["InternalApiKey"]
                      ?? throw new InvalidOperationException("InternalApiKey is not set");
            http.BaseAddress = new Uri(baseUrl);
            http.DefaultRequestHeaders.Add("X-Internal-Key", key);
        });

        return services;
    }
}
