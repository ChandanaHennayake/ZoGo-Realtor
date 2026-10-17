using Microsoft.EntityFrameworkCore;
using zogo.Domain.Entities.Identity;
using zogo.Domain.Entities.Master;

namespace zogo.Infrastructure.Persistence;

public static class MasterDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        try
        {
            if (!await context.Roles.AnyAsync(r => r.Code == "AGT"))
            {
                var agentRole = Role.Create("AGT", "Agent", true);
                await context.Roles.AddAsync(agentRole);
                await context.SaveChangesAsync();
            }

            if (!await context.Roles.AnyAsync(r => r.Code == "ADM"))
            {
                var adminRole = Role.Create("ADM", "Admin", true);
                await context.Roles.AddAsync(adminRole);
                await context.SaveChangesAsync();
            }

            if (!await context.Roles.AnyAsync(r => r.Code == "SEL"))
            {
                var sellerRole = Role.Create("SEL", "Seller", true);
                await context.Roles.AddAsync(sellerRole);
                await context.SaveChangesAsync();
            }

            if (!await context.Roles.AnyAsync(r => r.Code == "BYR"))
            {
                var buyerRole = Role.Create("BYR", "Buyer", true);
                await context.Roles.AddAsync(buyerRole);
                await context.SaveChangesAsync();
            }

            if (!await context.Features.AnyAsync())
            {
                var features = new[]
                {
                    Feature.Create("POOL", "Swimming Pool", "Outdoors", true, 1),
                    Feature.Create("GARDEN", "Garden", "Outdoors", true, 2),
                    Feature.Create("BALCONY", "Balcony", "General", true, 3),
                    Feature.Create("PARKING", "Parking", "General", true, 4),
                    Feature.Create("SECURITY", "Security", "Safety", true, 5),
                    Feature.Create("CCTV", "CCTV", "Safety", true, 6),
                    Feature.Create("AC", "Air Conditioning", "Comfort", true, 7),
                    Feature.Create("ELEVATOR", "Elevator", "Building", true, 8),
                    Feature.Create("GENERATOR", "Generator", "Utilities", true, 9),
                    Feature.Create("GYM", "Gym", "Fitness", true, 10),
                    Feature.Create("CLUBHOUSE", "Clubhouse", "Community", true, 11),
                    Feature.Create("PLAYGROUND", "Playground", "Community", true, 12),
                    Feature.Create("ROOFTOP", "Rooftop Terrace", "Outdoors", true, 13),
                    Feature.Create("WATERTANK", "Water Tank", "Utilities", true, 14),
                    Feature.Create("SOLAR", "Solar Power", "Utilities", true, 15),
                    Feature.Create("ROAD", "Road Access", "General", true, 16),
                    Feature.Create("FENCED", "Fenced", "Safety", true, 17)
                };
                await context.Features.AddRangeAsync(features);
                await context.SaveChangesAsync();
            }

            if (!await context.Amenities.AnyAsync())
            {
                var amenities = new[]
                {
                    Amenity.Create("POOL", "Swimming Pool", "Leisure", true, 1),
                    Amenity.Create("GYM", "Gym", "Fitness", true, 2),
                    Amenity.Create("SECURITY", "24/7 Security", "Safety", true, 3),
                    Amenity.Create("CLUBHOUSE", "Clubhouse", "Community", true, 4),
                    Amenity.Create("PARKING", "Covered Parking", "Parking", true, 5),
                    Amenity.Create("VISITOR_PARKING", "Visitor Parking", "Parking", true, 6),
                    Amenity.Create("PLAYGROUND", "Play Area", "Community", true, 7),
                    Amenity.Create("POWER_BACKUP", "Power Backup", "Utilities", true, 8),
                    Amenity.Create("ELEVATOR", "Elevator", "Building", true, 9),
                    Amenity.Create("CCTV", "CCTV Surveillance", "Safety", true, 10),
                    Amenity.Create("ROOFTOP", "Rooftop Terrace", "Leisure", true, 11),
                    Amenity.Create("GARDEN", "Garden", "Leisure", true, 12)
                };
                await context.Amenities.AddRangeAsync(amenities);
                await context.SaveChangesAsync();
            }

            if (!await context.ApartmentTypes.AnyAsync())
            {
                var apartmentTypes = new[]
                {
                    ApartmentType.Create("STUDIO", "Studio"),
                    ApartmentType.Create("1BHK", "1 Bedroom"),
                    ApartmentType.Create("2BHK", "2 Bedrooms"),
                    ApartmentType.Create("3BHK", "3 Bedrooms"),
                    ApartmentType.Create("4BHK", "4+ Bedrooms"),
                    ApartmentType.Create("PENTHOUSE", "Penthouse"),
                    ApartmentType.Create("DUPLEX", "Duplex")
                };
                await context.ApartmentTypes.AddRangeAsync(apartmentTypes);
                await context.SaveChangesAsync();
            }

            if (!await context.FurnishingTypes.AnyAsync())
            {
                var furnishingTypes = new[]
                {
                    FurnishingType.Create("UNFURNISHED", "Unfurnished"),
                    FurnishingType.Create("SEMI_FURNISHED", "Semi-Furnished"),
                    FurnishingType.Create("FULLY_FURNISHED", "Fully Furnished")
                };
                await context.FurnishingTypes.AddRangeAsync(furnishingTypes);
                await context.SaveChangesAsync();
            }

            // Seed default Admin user
            var adminEmail = "admin@zogo.com";
            if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
            {
                var adminUser = User.Create("ZoGo", "Admin", adminEmail, "0770000001");
                adminUser.MarkEmailAsVerified();
                adminUser.MarkPhoneAsVerified();
                var adminPwdHash = BCrypt.Net.BCrypt.HashPassword("Admin@12345", 12);
                var authProvider = UserAuthenticationProvider.CreateLocal(adminUser.Id, adminEmail, adminPwdHash);
                adminUser.AddAuthenticationProvider(authProvider);

                var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "ADM");
                if (adminRole is not null)
                {
                    adminUser.AddRole(UserRole.Create(adminUser.Id, adminRole.Id, null));
                }

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }

            // Seed default Agent user
            var agentEmail = "agent@zogo.com";
            if (!await context.Users.AnyAsync(u => u.Email == agentEmail))
            {
                var agentUser = User.Create("ZoGo", "Agent", agentEmail, "0770000002");
                agentUser.MarkEmailAsVerified();
                agentUser.MarkPhoneAsVerified();
                var agentPwdHash = BCrypt.Net.BCrypt.HashPassword("Agent@12345", 12);
                var authProvider = UserAuthenticationProvider.CreateLocal(agentUser.Id, agentEmail, agentPwdHash);
                agentUser.AddAuthenticationProvider(authProvider);

                var agentRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "AGT");
                if (agentRole is not null)
                {
                    agentUser.AddRole(UserRole.Create(agentUser.Id, agentRole.Id, null));
                }

                await context.Users.AddAsync(agentUser);
                await context.SaveChangesAsync();

                var agentProfile = zogo.Domain.Entities.Agent.AgentProfile.Create(agentUser.Id, 1, true);
                await context.AgentProfiles.AddAsync(agentProfile);
                await context.AgentDistricts.AddRangeAsync(new[]
                {
                    zogo.Domain.Entities.Agent.AgentDistrict.Create(agentProfile.Id, 1),
                    zogo.Domain.Entities.Agent.AgentDistrict.Create(agentProfile.Id, 2)
                });
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MasterDataSeeder] Error during seeding: {ex.Message}");
        }
    }
}
