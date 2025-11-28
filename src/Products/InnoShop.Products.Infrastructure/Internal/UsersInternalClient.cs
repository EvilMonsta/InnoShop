using System.Net.Http.Json;
using InnoShop.Products.Application.Abstractions;

namespace InnoShop.Products.Infrastructure.Internal;

public sealed class UsersInternalClient : IUsersInternalClient
{
    private readonly HttpClient _http;
    public UsersInternalClient(HttpClient http) => _http = http;

    private sealed record StatusDto(bool IsActive);

    public async Task<bool> IsActiveAsync(Guid userId, CancellationToken ct = default)
    {
        var dto = await _http.GetFromJsonAsync<StatusDto>($"/internal/users/{userId}/is-active", ct);
        return dto?.IsActive ?? false;
    }

}
