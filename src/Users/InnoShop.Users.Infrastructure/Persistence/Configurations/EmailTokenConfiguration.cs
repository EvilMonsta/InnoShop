using InnoShop.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoShop.Users.Infrastructure.Persistence.Configurations;

public class EmailTokenConfiguration : IEntityTypeConfiguration<EmailToken>
{
    public void Configure(EntityTypeBuilder<EmailToken> b)
    {
        b.ToTable("email_tokens");
        b.HasKey(x => x.Id);
        b.Property(x => x.TokenHash).IsRequired().HasMaxLength(88); 
        b.Property(x => x.Purpose).IsRequired();
        b.Property(x => x.ExpiresAt).IsRequired();
        b.Property(x => x.UsedAt);
        b.HasIndex(x => new { x.TokenHash, x.Purpose }).IsUnique();
        b.HasIndex(x => x.UserId);
    }
}
