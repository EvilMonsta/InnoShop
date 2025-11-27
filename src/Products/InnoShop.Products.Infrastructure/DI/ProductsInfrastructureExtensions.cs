using InnoShop.Products.Application.Abstractions;
using InnoShop.Products.Infrastructure.Persistence;
using InnoShop.Products.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.Products.Infrastructure.DI;

public static class ProductsInfrastructureExtensions
{
    public static IServiceCollection AddProductsInfrastructure(this IServiceCollection services, string? connectionString)
    {
        services.AddDbContext<ProductsDbContext>(opt => opt.UseNpgsql(connectionString));
        services.AddScoped<IProductRepository, ProductRepository>();
        return services;
    }
}