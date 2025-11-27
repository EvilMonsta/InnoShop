namespace InnoShop.Users.Domain.Events;

public sealed record UserDeactivated(Guid UserId, DateTimeOffset OccurredAt);
