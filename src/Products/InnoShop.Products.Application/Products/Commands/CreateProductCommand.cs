namespace InnoShop.Products.Application.Products.Commands;

public record CreateProductCommand(Guid OwnerUserId, string Name, string? Description, decimal Price);
