using InnoShop.Users.Application.Abstractions;
using InnoShop.Users.Contracts.Requests;
using InnoShop.Users.Contracts.Responses;
using InnoShop.Users.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InnoShop.Users.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IProductsInternalClient _productsInternal;

    private static string? GetUserId(ClaimsPrincipal u) =>
        u.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
        u.FindFirstValue(ClaimTypes.NameIdentifier) ??
        u.FindFirstValue("sub");
    public UsersController(IUserRepository users, IProductsInternalClient productsInternal)
    {
        _users = users;
        _productsInternal = productsInternal;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken ct)
    {
        var u = await _users.GetByIdAsync(id, ct);
        if (u is null) return NotFound();
        return new UserResponse(u.Id, u.Name, u.Email, u.Role.ToString(), u.IsActive, u.EmailConfirmed, u.CreatedAt.ToString("O"));
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest req, CancellationToken ct)
    {
        var callerId = GetUserId(User);
        var isAdmin = User.IsInRole("Admin");
        if (callerId is null) return Forbid();

        var isSelf = Guid.TryParse(callerId, out var callerGuid) && callerGuid == id;
        if (!isAdmin && !isSelf) return Forbid();

        var u = await _users.GetByIdAsync(id, ct);
        if (u is null) return NotFound();

        u.UpdateName(req.Name);
        if (!string.IsNullOrWhiteSpace(req.Role) && Enum.TryParse<Role>(req.Role, true, out var role))
            u.SetRole(role);

        await _users.UpdateAsync(u, ct);
        await _users.SaveChangesAsync(ct);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var callerId = GetUserId(User);
        var isAdmin = User.IsInRole("Admin");
        if (callerId is null) return Forbid();

        var isSelf = Guid.TryParse(callerId, out var callerGuid) && callerGuid == id;
        if (!isAdmin && !isSelf) return Forbid();

        var u = await _users.GetByIdAsync(id, ct);
        if (u is null) return NotFound();

        await _productsInternal.DeleteAllProductsAsync(id, ct);

        await _users.DeleteAsync(u, ct);
        await _users.SaveChangesAsync(ct);
        return NoContent();
    }



    [Authorize]
    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        var callerId = GetUserId(User);
        var isAdmin = User.IsInRole("Admin");

        if (callerId is null) return Forbid();

        var isSelf = Guid.TryParse(callerId, out var callerGuid) && callerGuid == id;
        if (!isAdmin && !isSelf) return Forbid();

        var u = await _users.GetByIdAsync(id, ct);
        if (u is null) return NotFound();

        u.Deactivate();
        await _users.UpdateAsync(u, ct);
        await _users.SaveChangesAsync(ct);

        await _productsInternal.HideProductsAsync(id, ct);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id:guid}/reactivate")]
    public async Task<IActionResult> Reactivate(Guid id, CancellationToken ct)
    {
        var callerId = GetUserId(User);
        var isAdmin = User.IsInRole("Admin");

        if (callerId is null) return Forbid();

        var isSelf = Guid.TryParse(callerId, out var callerGuid) && callerGuid == id;
        if (!isAdmin && !isSelf) return Forbid();

        var u = await _users.GetByIdAsync(id, ct);
        if (u is null) return NotFound();

        u.Reactivate();
        await _users.UpdateAsync(u, ct);
        await _users.SaveChangesAsync(ct);

        await _productsInternal.ShowProductsAsync(id, ct);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<UserResponse>> List(
       [FromQuery] string? q,
       [FromQuery] string? role,
       [FromQuery] bool? isActive,
       [FromQuery] bool? emailConfirmed,
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 50,
       CancellationToken ct = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 200 ? 50 : pageSize;

        var query = _users.Query();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var qc = q.ToLower();
            query = query.Where(u =>
                u.Name.ToLower().Contains(qc) ||
                u.Email.ToLower().Contains(qc));
        }

        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<Role>(role, true, out var r))
            query = query.Where(u => u.Role == r);

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        if (emailConfirmed.HasValue)
            query = query.Where(u => u.EmailConfirmed == emailConfirmed.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserResponse(
                u.Id, u.Name, u.Email, u.Role.ToString(), u.IsActive, u.EmailConfirmed, u.CreatedAt.ToString("O")))
            .ToListAsync(ct);

        return Ok(new InnoShop.Users.Contracts.Responses.PagedResult<UserResponse>(items, total, page, pageSize));
    }

}
