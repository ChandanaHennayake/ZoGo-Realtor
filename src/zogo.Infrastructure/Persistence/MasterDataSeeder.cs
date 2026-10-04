using Microsoft.EntityFrameworkCore;
using zogo.Domain.Entities.Master;

namespace zogo.Infrastructure.Persistence;

public static class MasterDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        try
        {
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MasterDataSeeder] Error during seeding: {ex.Message}");
        }
    }
}
