namespace zogo.Domain.Entities.Master;

public class Condominium
{
    private Condominium()
    {
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Address { get; private set; }

    public short? DistrictId { get; private set; }

    public string? DeveloperName { get; private set; }

    public int? TotalFloors { get; private set; }

    public decimal? Latitude { get; private set; }

    public decimal? Longitude { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public static Condominium Create(
        string name,
        string? address,
        short? districtId,
        string? developerName,
        int? totalFloors,
        decimal? latitude,
        decimal? longitude)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Condominium name is required.",
                nameof(name));

        if (districtId.HasValue && districtId.Value <= 0)
            throw new ArgumentException(
                "District ID must be greater than zero.",
                nameof(districtId));

        if (totalFloors.HasValue && totalFloors.Value <= 0)
            throw new ArgumentException(
                "Total floors must be greater than zero.",
                nameof(totalFloors));

        return new Condominium
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Address = string.IsNullOrWhiteSpace(address)
                ? null
                : address.Trim(),
            DistrictId = districtId,
            DeveloperName = string.IsNullOrWhiteSpace(developerName)
                ? null
                : developerName.Trim(),
            TotalFloors = totalFloors,
            Latitude = latitude,
            Longitude = longitude,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        string? address,
        short? districtId,
        string? developerName,
        int? totalFloors,
        decimal? latitude,
        decimal? longitude)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Condominium name is required.",
                nameof(name));

        if (districtId.HasValue && districtId.Value <= 0)
            throw new ArgumentException(
                "District ID must be greater than zero.",
                nameof(districtId));

        if (totalFloors.HasValue && totalFloors.Value <= 0)
            throw new ArgumentException(
                "Total floors must be greater than zero.",
                nameof(totalFloors));

        Name = name.Trim();

        Address = string.IsNullOrWhiteSpace(address)
            ? null
            : address.Trim();

        DistrictId = districtId;

        DeveloperName = string.IsNullOrWhiteSpace(developerName)
            ? null
            : developerName.Trim();

        TotalFloors = totalFloors;
        Latitude = latitude;
        Longitude = longitude;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}