using Microsoft.EntityFrameworkCore;
using zogo.Application.Interfaces.Repositories;
using zogo.Domain.Entities.Agent;
using zogo.Domain.Entities.Buyer;
using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;
using zogo.Domain.Entities.Notification;
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

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<PasswordReset> PasswordResets => Set<PasswordReset>();

    public DbSet<DeviceToken> DeviceTokens => Set<DeviceToken>();

    // =========================================================
    // Property
    // =========================================================

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<District> Districts => Set<District>();

    public DbSet<Province> Provinces => Set<Province>();

    public DbSet<City> Cities => Set<City>();
    public DbSet<DivisionalSecretariat> DivisionalSecretariats => Set<DivisionalSecretariat>();
    public DbSet<GramaNiladhariDivision> GramaNiladhariDivisions => Set<GramaNiladhariDivision>();
    public DbSet<PropertyType> PropertyTypes => Set<PropertyType>();
    public DbSet<ListingType> ListingTypes => Set<ListingType>();
    public DbSet<PropertyStatus> PropertyStatuses => Set<PropertyStatus>();

    public DbSet<Condominium> Condominiums => Set<Condominium>();
    public DbSet<ApartmentType> ApartmentTypes => Set<ApartmentType>();
    public DbSet<FurnishingType> FurnishingTypes => Set<FurnishingType>();

    public DbSet<ApartmentDetails> ApartmentDetails => Set<ApartmentDetails>();

    public DbSet<PropertyFinancials> PropertyFinancials => Set<PropertyFinancials>();
    public DbSet<PropertyLegalDetails> PropertyLegalDetails => Set<PropertyLegalDetails>();

    public DbSet<Feature> Features => Set<Feature>();

    public DbSet<PropertyFeature> PropertyFeatures { get; set; } = null!;

    public DbSet<Amenity> Amenities { get; set; } = null!;

    public DbSet<PropertyAmenity> PropertyAmenities { get; set; } = null!;

    public DbSet<PropertyMedia> PropertyMedia { get; set; } = null!;
    public DbSet<PropertyDocument> PropertyDocuments { get; set; } = null!;

    // =========================================================
    // Buyer
    // =========================================================

    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<PropertyInterest> PropertyInterests => Set<PropertyInterest>();
    public DbSet<BuyerPropertyInteraction> BuyerPropertyInteractions => Set<BuyerPropertyInteraction>();
    public DbSet<PropertyVisit> PropertyVisits => Set<PropertyVisit>();
    public DbSet<PropertyOffer> PropertyOffers => Set<PropertyOffer>();
    public DbSet<PropertyTransaction> PropertyTransactions => Set<PropertyTransaction>();

    // =========================================================
    // Agent & Notifications
    // =========================================================

    public DbSet<AgentProfile> AgentProfiles => Set<AgentProfile>();
    public DbSet<AgentDistrict> AgentDistricts => Set<AgentDistrict>();
    public DbSet<AgentNotification> AgentNotifications => Set<AgentNotification>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

        base.OnConfiguring(optionsBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<NullableUtcDateTimeConverter>();
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

public class UtcDateTimeConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter()
        : base(
            v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}

public class NullableUtcDateTimeConverter : Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>
{
    public NullableUtcDateTimeConverter()
        : base(
            v => v.HasValue ? (v.Value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v.Value.ToUniversalTime()) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v)
    {
    }
}
