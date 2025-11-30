using InnoShop.Users.Application.Abstractions;
using InnoShop.Users.Domain.Users;
using InnoShop.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace InnoShop.Users.Infrastructure.Repositories;


public class UserRepository : IUserRepository
{
    private readonly UsersDbContext _db;
    public UserRepository(UsersDbContext db) => _db = db;


    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default)
    => _db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);


    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    => _db.Users.FirstOrDefaultAsync(x => x.Email == email, ct);


    public async Task AddAsync(User user, CancellationToken ct = default)
    => await _db.Users.AddAsync(user, ct);


    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        return Task.CompletedTask;
    }


    public Task DeleteAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Remove(user);
        return Task.CompletedTask;
    }
    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    public IQueryable<User> Query() => _db.Users.AsQueryable();

}