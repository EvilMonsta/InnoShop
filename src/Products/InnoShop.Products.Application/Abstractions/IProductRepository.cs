using InnoShop.Products.Domain.Products;

namespace InnoShop.Products.Application.Abstractions;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    IQueryable<Product> Query();
    Task AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<int> HideByOwnerAsync(Guid ownerUserId, CancellationToken ct = default);
    Task<int> ShowByOwnerAsync(Guid ownerUserId, CancellationToken ct = default);
}