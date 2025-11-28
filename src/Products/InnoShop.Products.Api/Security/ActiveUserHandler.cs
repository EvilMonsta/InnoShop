using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using InnoShop.Products.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace InnoShop.Products.Api.Security;

public sealed class ActiveUserHandler : AuthorizationHandler<ActiveUserRequirement>
{
    private readonly IUsersInternalClient _usersClient;

    public ActiveUserHandler(IUsersInternalClient usersClient)
        => _usersClient = usersClient;

    private static string? GetUserId(ClaimsPrincipal u) =>
        u.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
        u.FindFirstValue(ClaimTypes.NameIdentifier) ??
        u.FindFirstValue("sub");

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ActiveUserRequirement requirement)
    {
        var userId = GetUserId(context.User);
        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out var id))
        {
            context.Fail();
            return;
        }

        try
        {
            var isActive = await _usersClient.IsActiveAsync(id);
            if (isActive)
                context.Succeed(requirement);
            else
                context.Fail();
        }
        catch
        {
            context.Fail();
        }
    }
}
