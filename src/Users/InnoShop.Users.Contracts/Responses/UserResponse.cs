namespace InnoShop.Users.Contracts.Responses;


public record UserResponse(Guid Id, string Name, string Email, string Role, bool IsActive, bool EmailConfirmed, string CreatedAt);