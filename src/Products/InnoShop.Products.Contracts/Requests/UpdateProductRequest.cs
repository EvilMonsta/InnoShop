namespace InnoShop.Products.Contracts.Requests;

public record UpdateProductRequest(string Name, string? Description, decimal Price, bool IsAvailable);