using InnoShop.Users.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Users.Api.Controllers;

[ApiController]
[Route("internal/users")]
public class InternalUsersStatusController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IConfiguration _cfg;

    public InternalUsersStatusController(IUserRepository users, IConfiguration cfg)
    {
        _users = users;
        _cfg = cfg;
    }

    private bool CheckInternalKey()
    {
        var ok = Request.Headers.TryGetValue("X-Internal-Key", out var got);
        var expected = _cfg["InternalApiKey"];
        return ok && !string.IsNullOrWhiteSpace(expected) && got == expected;
    }

    [HttpGet("{id:guid}/is-active")]
    public async Task<ActionResult<object>> IsActive(Guid id, CancellationToken ct)
    {
        if (!CheckInternalKey()) return Unauthorized();
        var u = await _users.GetByIdAsync(id, ct);
        if (u is null) return NotFound();
        return Ok(new { isActive = u.IsActive });
    }
}

