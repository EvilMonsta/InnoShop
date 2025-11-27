namespace InnoShop.Users.Contracts.Responses;


public record AuthResponse(string AccessToken, string TokenType = "Bearer");