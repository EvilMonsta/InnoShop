using InnoShop.Products.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Products.Infrastructure.Persistence;

public class ProductsDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductsDbContext).Assembly);
    }
}