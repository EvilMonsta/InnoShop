namespace InnoShop.Users.Contracts.Responses;

public record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);