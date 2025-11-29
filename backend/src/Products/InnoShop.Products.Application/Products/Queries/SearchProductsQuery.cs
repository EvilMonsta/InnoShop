namespace InnoShop.Products.Application.Products.Queries;

public record SearchProductsQuery(string? Q = null, decimal? MinPrice = null, decimal? MaxPrice = null, bool? OnlyAvailable = null, Guid? OwnerUserId = null, int Page = 1, int PageSize = 50);