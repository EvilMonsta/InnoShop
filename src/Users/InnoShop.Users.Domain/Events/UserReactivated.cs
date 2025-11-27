namespace InnoShop.Users.Domain.Events;

public sealed record UserReactivated(Guid UserId, DateTimeOffset OccurredAt);
