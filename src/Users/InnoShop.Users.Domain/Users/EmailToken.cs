namespace InnoShop.Users.Domain.Users;

public class EmailToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = default!; 
    public EmailTokenPurpose Purpose { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? UsedAt { get; private set; }

    private EmailToken() { }

    public EmailToken(Guid userId, string tokenHash, EmailTokenPurpose purpose, DateTimeOffset expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        Purpose = purpose;
        ExpiresAt = expiresAt;
    }

    public bool IsUsed => UsedAt.HasValue;
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    public void MarkUsed()
    {
        if (!IsUsed) UsedAt = DateTimeOffset.UtcNow;
    }
}
