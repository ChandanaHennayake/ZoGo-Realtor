using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Property;

namespace zogo.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<UserAuthenticationProvider> UserAuthenticationProviders => Set<UserAuthenticationProvider>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<UserRole> UserRoles =>Set<UserRole>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<PasswordReset> PasswordResets =>Set<PasswordReset>();

    public DbSet<DeviceToken> DeviceTokens => Set<DeviceToken>();

    // =========================================================
    // Property
    // =========================================================

    public DbSet<Property> Properties =>Set<Property>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<DivisionalSecretariat> DivisionalSecretariats => Set<DivisionalSecretariat>();
    public DbSet<GramaNiladhariDivision> GramaNiladhariDivisions => Set<GramaNiladhariDivision>();
    public DbSet<PropertyType> PropertyTypes => Set<PropertyType>();
    public DbSet<ListingType> ListingTypes => Set<ListingType>();



    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }


   
}