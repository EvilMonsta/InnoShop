using InnoShop.Users.Application.Abstractions;

namespace InnoShop.Users.Infrastructure.Integration;

public class ProductsInternalClient : IProductsInternalClient
{
    private readonly HttpClient _http;

    public ProductsInternalClient(HttpClient http) => _http = http;

    public Task HideProductsAsync(Guid ownerUserId, CancellationToken ct = default)
        => _http.PostAsync($"/internal/users/{ownerUserId}/hide-products", content: null, ct);

    public Task ShowProductsAsync(Guid ownerUserId, CancellationToken ct = default)
        => _http.PostAsync($"/internal/users/{ownerUserId}/show-products", content: null, ct);
    
    public async Task DeleteAllProductsAsync(Guid userId, CancellationToken ct = default)
    {
        var resp = await _http.DeleteAsync($"/internal/users/{userId}/products", ct);
        resp.EnsureSuccessStatusCode();
    }

}
