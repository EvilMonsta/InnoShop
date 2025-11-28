using InnoShop.Products.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Products.Api.Controllers;

[ApiController]
[Route("internal/users")]
public class InternalUsersProductsController : ControllerBase
{
    private readonly ProductsDbContext _db;
    private readonly IConfiguration _cfg;
    public InternalUsersProductsController(ProductsDbContext db, IConfiguration cfg)
        => (_db, _cfg) = (db, cfg);

    private bool CheckKey() =>
        Request.Headers.TryGetValue("X-Internal-Key", out var got) &&
        !string.IsNullOrWhiteSpace(_cfg["InternalApiKey"]) &&
        got == _cfg["InternalApiKey"];

    [HttpDelete("{userId:guid}/products")]
    public async Task<IActionResult> DeleteAll(Guid userId, CancellationToken ct)
    {
        if (!CheckKey()) return Unauthorized();

        await _db.Products
            .Where(p => p.OwnerUserId == userId && !p.IsDeleted)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.IsDeleted, true)
                .SetProperty(p => p.IsHiddenByOwnerDeactivation, false), ct);

        return NoContent();
    }
}
