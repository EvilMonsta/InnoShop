using InnoShop.Products.Application.Abstractions;
using InnoShop.Products.Domain.Products;
using InnoShop.Products.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Products.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductsDbContext _db;
    public ProductRepository(ProductsDbContext db) => _db = db;

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);

    public IQueryable<Product> Query() => _db.Products.AsQueryable();

    public Task AddAsync(Product product, CancellationToken ct = default)
        => _db.Products.AddAsync(product, ct).AsTask();

    public Task UpdateAsync(Product product, CancellationToken ct = default)
    { _db.Products.Update(product); return Task.CompletedTask; }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public Task<int> HideByOwnerAsync(Guid ownerUserId, CancellationToken ct = default)
        => _db.Products
              .IgnoreQueryFilters()
              .Where(p => p.OwnerUserId == ownerUserId && !p.IsDeleted && !p.IsHiddenByOwnerDeactivation)
              .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsHiddenByOwnerDeactivation, true), ct);

    public Task<int> ShowByOwnerAsync(Guid ownerUserId, CancellationToken ct = default)
        => _db.Products
              .IgnoreQueryFilters()
              .Where(p => p.OwnerUserId == ownerUserId && !p.IsDeleted && p.IsHiddenByOwnerDeactivation)
              .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsHiddenByOwnerDeactivation, false), ct);
}