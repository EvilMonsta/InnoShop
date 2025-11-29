using InnoShop.Users.Domain.Users;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace InnoShop.Users.Infrastructure.Persistence;


public class UsersDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<EmailToken> EmailTokens => Set<EmailToken>();

    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
    }
}