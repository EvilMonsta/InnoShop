using InnoShop.Products.Application.Abstractions;

namespace InnoShop.Products.Application.Integration.Handlers;

public readonly record struct UserReactivated(Guid UserId);

public sealed class UserReactivatedHandler
{
    private readonly IProductRepository _repo;

    public UserReactivatedHandler(IProductRepository repo) => _repo = repo;

    public Task<int> HandleAsync(UserReactivated evt, CancellationToken ct = default)
        => _repo.ShowByOwnerAsync(evt.UserId, ct);
}
