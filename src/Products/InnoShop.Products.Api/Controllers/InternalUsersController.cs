using InnoShop.Products.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Products.Api.Controllers;

[ApiController]
[Route("internal/users")]
public class InternalUsersController : ControllerBase
{
    private readonly IProductRepository _repo;
    private readonly IConfiguration _cfg;

    public InternalUsersController(IProductRepository repo, IConfiguration cfg)
    {
        _repo = repo; _cfg = cfg;
    }

    private bool CheckInternalKey()
    {
        var ok = Request.Headers.TryGetValue("X-Internal-Key", out var got);
        var expected = _cfg["InternalApiKey"];
        return ok && !string.IsNullOrWhiteSpace(expected) && got == expected;
    }

    [HttpPost("{id:guid}/hide-products")]
    public async Task<IActionResult> Hide(Guid id, CancellationToken ct)
    {
        if (!CheckInternalKey()) return Unauthorized();

        var items = await _repo.GetByOwnerIgnoringFiltersAsync(id, ct);
        foreach (var p in items.Where(p => !p.IsDeleted))
            p.HideByOwner();

        await _repo.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/show-products")]
    public async Task<IActionResult> Show(Guid id, CancellationToken ct)
    {
        if (!CheckInternalKey()) return Unauthorized();

        var items = await _repo.GetByOwnerIgnoringFiltersAsync(id, ct);
        foreach (var p in items.Where(p => !p.IsDeleted && p.IsHiddenByOwnerDeactivation))
            p.ShowByOwner();

        await _repo.SaveChangesAsync(ct);
        return NoContent();
    }
}
