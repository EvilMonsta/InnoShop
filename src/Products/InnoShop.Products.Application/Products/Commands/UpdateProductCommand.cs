namespace InnoShop.Products.Application.Products.Commands;

public record UpdateProductCommand(Guid Id, string Name, string? Description, decimal Price, bool IsAvailable);
