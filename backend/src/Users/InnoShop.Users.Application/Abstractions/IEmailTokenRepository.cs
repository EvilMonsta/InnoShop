using InnoShop.Users.Domain.Users;

namespace InnoShop.Users.Application.Abstractions;

public interface IEmailTokenRepository
{
    Task AddAsync(EmailToken token, CancellationToken ct = default);
    Task<EmailToken?> GetValidAsync(string tokenHash, EmailTokenPurpose purpose, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<int> PurgeExpiredAsync(CancellationToken ct = default);
}
