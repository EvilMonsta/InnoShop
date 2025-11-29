using InnoShop.Products.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InnoShop.Products.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("products");
        b.HasKey(x => x.Id);
        b.Property(x => x.OwnerUserId).IsRequired();
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Price).HasColumnType("numeric(18,2)");
        b.HasIndex(x => x.OwnerUserId);
        b.HasQueryFilter(p => !p.IsDeleted && !p.IsHiddenByOwnerDeactivation);
    }
}