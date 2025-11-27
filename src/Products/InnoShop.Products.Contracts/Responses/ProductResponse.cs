namespace InnoShop.Products.Contracts.Responses;

public record ProductResponse(Guid Id, Guid OwnerUserId, string Name, string? Description, decimal Price, bool IsAvailable, string CreatedAt);