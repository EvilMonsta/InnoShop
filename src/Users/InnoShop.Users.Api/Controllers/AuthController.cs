using InnoShop.Users.Application.Abstractions;
using InnoShop.Users.Contracts.Requests;
using InnoShop.Users.Contracts.Responses;
using InnoShop.Users.Domain.Users;
using InnoShop.Users.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;


namespace InnoShop.Users.Api.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokens;
    private readonly IEmailSender _email;


    public AuthController(IUserRepository users, ITokenService tokens, IEmailSender email)
    {
        _users = users; _tokens = tokens; _email = email;
    }


    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterUserRequest req, CancellationToken ct)
    {
       
        var existing = await _users.GetByEmailAsync(req.Email, ct);
        if (existing is not null) return Conflict("Email already exists");


        var hash = PasswordHash.Hash(req.Password);
        var user = new User(req.Name, req.Email, Role.User, hash);
        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);


        await _email.SendAsync(user.Email, "Confirm your account", $"Confirm via: /api/auth/confirm-email?userId={user.Id}", ct);


        return CreatedAtAction(nameof(Register), new UserResponse(user.Id, user.Name, user.Email, user.Role.ToString(), user.IsActive, user.EmailConfirmed, user.CreatedAt.ToString("O")));
    }


    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(req.Email, ct);
        if (user is null) return Unauthorized();
        if (!PasswordHash.Verify(req.Password, user.PasswordHash)) return Unauthorized();
        if (!user.IsActive) return Forbid();
        var token = _tokens.GenerateAccessToken(user);
        return new AuthResponse(token);
    }


    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestReset([FromBody] RequestPasswordResetRequest req, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(req.Email, ct);
        if (user is not null)
        {
            await _email.SendAsync(user.Email, "Reset password", $"Reset via: /api/auth/confirm-password-reset?userId={user.Id}", ct);
        }
        return Ok();
    }


    [HttpPost("confirm-password-reset")]
    public async Task<IActionResult> ConfirmReset([FromBody] ConfirmPasswordResetRequest req, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(req.UserId, ct);
        if (user is null) return NotFound();
        user.SetPasswordHash(PasswordHash.Hash(req.NewPassword));
        await _users.UpdateAsync(user, ct);
        await _users.SaveChangesAsync(ct);
        return NoContent();
    }


    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest req, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(req.UserId, ct);
        if (user is null) return NotFound();
        user.ConfirmEmail();
        await _users.UpdateAsync(user, ct);
        await _users.SaveChangesAsync(ct);
        return NoContent();
    }
}