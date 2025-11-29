namespace InnoShop.Products.Domain.Products;
public class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OwnerUserId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public bool IsAvailable { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public bool IsHiddenByOwnerDeactivation { get; private set; } = false;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    private Product() { }
    public Product(Guid ownerUserId, string name, string? description, decimal price)
    {
        OwnerUserId = ownerUserId;
        Name = name;
        Description = description;
        Price = price;
    }
    public void Update(string name, string? description, decimal price, bool isAvailable)
    {
        Name = name; Description = description; Price = price; IsAvailable = isAvailable;
    }
    public void SoftDelete() => IsDeleted = true;
    public void HideByOwner() => IsHiddenByOwnerDeactivation = true;
    public void ShowByOwner() => IsHiddenByOwnerDeactivation = false;
}