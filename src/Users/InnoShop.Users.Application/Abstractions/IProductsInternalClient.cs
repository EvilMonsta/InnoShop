namespace InnoShop.Users.Application.Abstractions;

public interface IProductsInternalClient
{
    Task HideProductsAsync(Guid ownerUserId, CancellationToken ct = default);
    Task ShowProductsAsync(Guid ownerUserId, CancellationToken ct = default);
    Task DeleteAllProductsAsync(Guid userId, CancellationToken ct = default);

}
