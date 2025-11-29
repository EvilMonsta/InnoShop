using InnoShop.Products.Application.Abstractions;
using InnoShop.Products.Infrastructure.Internal;
using InnoShop.Products.Infrastructure.Persistence;
using InnoShop.Products.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace InnoShop.Products.Infrastructure.DI;

public static class ProductsInfrastructureExtensions
{
    public static IServiceCollection AddProductsInfrastructure(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<ProductsDbContext>(opt => opt.UseNpgsql(connectionString));
        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddSingleton<IUsersInternalClient>(sp =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>();
            var env = sp.GetRequiredService<IHostEnvironment>();

            var baseUrl = cfg["UsersInternal:BaseUrl"]
                          ?? cfg["UsersInternal__BaseUrl"]  
                          ?? throw new InvalidOperationException("UsersInternal:BaseUrl is not set");
            var apiKey = cfg["InternalApiKey"]
                          ?? throw new InvalidOperationException("InternalApiKey is not set");

            var handler = new HttpClientHandler();

            var http = new HttpClient(handler) { BaseAddress = new Uri(baseUrl) };
            http.DefaultRequestHeaders.Add("X-Internal-Key", apiKey);
            http.Timeout = TimeSpan.FromSeconds(10);

            return new UsersInternalClient(http);
        });

        return services;
    }
}