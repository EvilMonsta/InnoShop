using InnoShop.Products.Application.Abstractions;

namespace InnoShop.Products.Application.Integration.Handlers;

public readonly record struct UserDeactivated(Guid UserId);

public sealed class UserDeactivatedHandler
{
    private readonly IProductRepository _repo;

    public UserDeactivatedHandler(IProductRepository repo) => _repo = repo;

    public Task<int> HandleAsync(UserDeactivated evt, CancellationToken ct = default)
        => _repo.HideByOwnerAsync(evt.UserId, ct);
}
