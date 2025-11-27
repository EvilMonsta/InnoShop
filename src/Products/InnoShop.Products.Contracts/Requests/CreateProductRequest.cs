namespace InnoShop.Products.Contracts.Requests;

public record CreateProductRequest(string Name, string? Description, decimal Price, bool IsAvailable = true);