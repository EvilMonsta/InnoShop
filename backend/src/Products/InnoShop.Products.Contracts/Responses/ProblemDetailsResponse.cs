namespace InnoShop.Products.Contracts.Responses;

public record ProblemDetailsResponse(string Type, string Title, int Status, string Detail, string Instance);