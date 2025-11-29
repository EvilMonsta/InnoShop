using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InnoShop.Users.Application.Abstractions;
using InnoShop.Users.Contracts.Requests;
using InnoShop.Users.Contracts.Responses;
using InnoShop.Users.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

}
