namespace InnoShop.Users.Contracts.Requests;


public record UpdateUserRequest(string Name, string? Role = null);