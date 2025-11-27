using System.Data;

namespace InnoShop.Users.Domain.Users;


public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public string Email { get; private set; }
    public Role Role { get; private set; } = Role.User;
    public bool IsActive { get; private set; } = true;
    public bool EmailConfirmed { get; private set; } = false;
    public string PasswordHash { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;


    private User() { }


    public User(string name, string email, Role role, string passwordHash)
    {
        Name = name;
        Email = email;
        Role = role;
        PasswordHash = passwordHash;
    }


    public void UpdateName(string name) => Name = name;
    public void SetRole(Role role) => Role = role;
    public void ConfirmEmail() => EmailConfirmed = true;
    public void Deactivate() => IsActive = false;
    public void Reactivate() => IsActive = true;
    public void SetPasswordHash(string hash) => PasswordHash = hash;
}