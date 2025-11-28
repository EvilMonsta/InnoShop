namespace InnoShop.Products.Application.Abstractions;

public interface IUsersInternalClient
{
    Task<bool> IsActiveAsync(Guid userId, CancellationToken ct = default);
}
