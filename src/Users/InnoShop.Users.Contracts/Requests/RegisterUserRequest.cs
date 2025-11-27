namespace InnoShop.Users.Contracts.Requests;


public record RegisterUserRequest(string Name, string Email, string Password);