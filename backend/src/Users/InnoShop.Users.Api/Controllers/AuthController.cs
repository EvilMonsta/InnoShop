using InnoShop.Users.Application.Abstractions;
using InnoShop.Users.Contracts.Requests;
using InnoShop.Users.Contracts.Responses;
using InnoShop.Users.Domain.Users;
using InnoShop.Users.Domain.ValueObjects;
using InnoShop.Users.Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Users.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly IEmailTokenRepository _tokens;
    private readonly ITokenService _jwt;
    private readonly IEmailSender _email;
    private readonly IConfiguration _cfg;

    public AuthController(
        IUserRepository users,
        IEmailTokenRepository tokens,
        ITokenService jwt,
        IEmailSender email,
        IConfiguration cfg)
    {
        _users = users;
        _tokens = tokens;
        _jwt = jwt;
        _email = email;
        _cfg = cfg;
    }


    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterUserRequest req, CancellationToken ct)
    {
        _ = new Email(req.Email);
        var existing = await _users.GetByEmailAsync(req.Email, ct);
        if (existing is not null) return Conflict("Email already exists");

        var hash = PasswordHash.Hash(req.Password);
        var user = new User(req.Name, req.Email, Role.User, hash);
        await _users.AddAsync(user, ct);
        await _users.SaveChangesAsync(ct);

        await SendEmailConfirmationAsync(user, ct);

        return CreatedAtAction(nameof(Register),
            new UserResponse(user.Id, user.Name, user.Email, user.Role.ToString(),
                             user.IsActive, user.EmailConfirmed, user.CreatedAt.ToString("O")));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(req.Email, ct);
        if (user is null) return Unauthorized();
        if (!PasswordHash.Verify(req.Password, user.PasswordHash)) return Unauthorized();
        if (!user.IsActive) return Forbid();
        var token = _jwt.GenerateAccessToken(user);
        return new AuthResponse(token);
    }


    [HttpPost("request-email-confirmation")]
    public async Task<IActionResult> RequestEmailConfirmation([FromBody] ConfirmEmailRequest req, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(req.UserId, ct);
        if (user is null) return NotFound();
        if (user.EmailConfirmed) return NoContent();

        await SendEmailConfirmationAsync(user, ct);
        return NoContent();
    }

    [HttpGet("confirm-email/{token}")]
    public async Task<IActionResult> ConfirmEmailByToken([FromRoute] string token, CancellationToken ct)
    {
        var tokenHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
        var t = await _tokens.GetValidAsync(tokenHash, EmailTokenPurpose.EmailConfirmation, ct);
        if (t is null) return BadRequest("Invalid or expired token");

        var user = await _users.GetByIdAsync(t.UserId, ct);
        if (user is null) return NotFound();

        user.ConfirmEmail();
        await _users.UpdateAsync(user, ct);
        t.MarkUsed();
        await _tokens.SaveChangesAsync(ct);
        await _users.SaveChangesAsync(ct);
        return NoContent();
    }


    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestReset([FromBody] RequestPasswordResetRequest req, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(req.Email, ct);
        if (user is not null)
        {
            var (raw, hash) = TokenGenerator.CreateSecureToken();
            var token = new EmailToken(user.Id, hash, EmailTokenPurpose.PasswordReset, DateTimeOffset.UtcNow.AddHours(1));
            await _tokens.AddAsync(token, ct);
            await _tokens.SaveChangesAsync(ct);

            var link = BuildPublicOrApiLink("reset", raw, fallbackApiPath: $"api/auth/reset-password/{raw}");
            var body = $@"<p>Для смены пароля перейдите по ссылке:</p>
                          <p><a href=""{link}"">{link}</a></p>
                          <p>Ссылка действительна 1 час.</p>";
            await _email.SendAsync(user.Email, "Reset your password", body, ct);
        }
        return Ok();
    }

    [HttpPost("reset-password/{token}")]
    public async Task<IActionResult> ResetPassword([FromRoute] string token, [FromBody] ConfirmPasswordResetRequest req, CancellationToken ct)
    {
        var tokenHash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(token)));
        var t = await _tokens.GetValidAsync(tokenHash, EmailTokenPurpose.PasswordReset, ct);
        if (t is null) return BadRequest("Invalid or expired token");

        var user = await _users.GetByIdAsync(t.UserId, ct);
        if (user is null) return NotFound();

        user.SetPasswordHash(PasswordHash.Hash(req.NewPassword));
        await _users.UpdateAsync(user, ct);
        t.MarkUsed();
        await _tokens.SaveChangesAsync(ct);
        await _users.SaveChangesAsync(ct);
        return NoContent();
    }


    private async Task SendEmailConfirmationAsync(User user, CancellationToken ct)
    {
        var (raw, hash) = TokenGenerator.CreateSecureToken();
        var token = new EmailToken(user.Id, hash, EmailTokenPurpose.EmailConfirmation, DateTimeOffset.UtcNow.AddDays(1));
        await _tokens.AddAsync(token, ct);
        await _tokens.SaveChangesAsync(ct);

        var link = BuildPublicOrApiLink("confirm", raw, fallbackApiPath: $"api/auth/confirm-email/{raw}");
        var body = $@"<p>Подтвердите e-mail, перейдя по ссылке:</p>
                      <p><a href=""{link}"">{link}</a></p>
                      <p>Ссылка действительна 24 часа.</p>";
        await _email.SendAsync(user.Email, "Confirm your email", body, ct);
    }

    private string BuildPublicOrApiLink(string publicPathSegment, string token, string fallbackApiPath)
    {
        var publicBase = _cfg["PublicBaseUrl"] ?? _cfg["Frontend:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(publicBase))
            return $"{publicBase!.TrimEnd('/')}/{publicPathSegment}/{token}";

        var apiBase = _cfg["ApiBaseUrl"];
        return string.IsNullOrWhiteSpace(apiBase)
            ? $"/{fallbackApiPath}"
            : $"{apiBase!.TrimEnd('/')}/{fallbackApiPath}";
    }
}
