using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Identity;

namespace zogo.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<UserAuthenticationProvider>
        UserAuthenticationProviders =>
        Set<UserAuthenticationProvider>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<UserRole> UserRoles =>
        Set<UserRole>();

    public DbSet<RefreshToken> RefreshTokens =>
        Set<RefreshToken>();

    public DbSet<PasswordReset> PasswordResets =>
        Set<PasswordReset>();

    public DbSet<DeviceToken> DeviceTokens =>
        Set<DeviceToken>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}