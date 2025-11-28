using InnoShop.Users.Application.Abstractions;
using InnoShop.Users.Domain.Users;
using InnoShop.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Users.Infrastructure.Repositories;

public class EmailTokenRepository : IEmailTokenRepository
{
    private readonly UsersDbContext _db;
    public EmailTokenRepository(UsersDbContext db) => _db = db;

    public Task AddAsync(EmailToken token, CancellationToken ct = default)
        => _db.EmailTokens.AddAsync(token, ct).AsTask();

    public Task<EmailToken?> GetValidAsync(string tokenHash, EmailTokenPurpose purpose, CancellationToken ct = default)
        => _db.EmailTokens.FirstOrDefaultAsync(x =>
               x.TokenHash == tokenHash
            && x.Purpose == purpose
            && x.UsedAt == null
            && x.ExpiresAt > DateTimeOffset.UtcNow, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => _db.SaveChangesAsync(ct);

    public async Task<int> PurgeExpiredAsync(CancellationToken ct = default)
    {
        var expired = await _db.EmailTokens
            .Where(x => x.ExpiresAt <= DateTimeOffset.UtcNow || x.UsedAt != null)
            .ExecuteDeleteAsync(ct);
        return expired;
    }
}
