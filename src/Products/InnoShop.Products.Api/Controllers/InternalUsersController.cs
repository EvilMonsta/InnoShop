using InnoShop.Products.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Products.Api.Controllers;

[ApiController]
[Route("internal/users")]
public class InternalUsersController : ControllerBase
{
    private readonly IProductRepository _repo;
    private readonly IConfiguration _cfg;

    public InternalUsersController(IProductRepository repo, IConfiguration cfg)
    {
        _repo = repo;
        _cfg = cfg;
    }

    private bool CheckInternalKey(HttpRequest req)
    {
        var expected = _cfg["InternalApiKey"] ?? "dev-internal";
        return req.Headers.TryGetValue("X-Internal-Key", out var val) && val == expected;
    }

    [HttpPost("{ownerId:guid}/hide-products")]
    public async Task<IActionResult> HideProducts(Guid ownerId, CancellationToken ct)
    {
        if (!CheckInternalKey(Request)) return Unauthorized();
        await _repo.HideByOwnerAsync(ownerId, ct);
        await _repo.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{ownerId:guid}/show-products")]
    public async Task<IActionResult> ShowProducts(Guid ownerId, CancellationToken ct)
    {
        if (!CheckInternalKey(Request)) return Unauthorized();
        await _repo.ShowByOwnerAsync(ownerId, ct);
        await _repo.SaveChangesAsync(ct);
        return NoContent();
    }
}
