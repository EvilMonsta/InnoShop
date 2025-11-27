namespace InnoShop.Users.Contracts.Responses;


public record ProblemDetailsResponse(string Type, string Title, int Status, string Detail, string Instance);