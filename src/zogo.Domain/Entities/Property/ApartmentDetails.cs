namespace zogo.Domain.Entities.Property;

public class ApartmentDetails
{
    private ApartmentDetails()
    {
    }

    public Guid PropertyId { get; private set; }

    public Guid? CondominiumId { get; private set; }

    public short? ApartmentTypeId { get; private set; }

    public int? FloorNumber { get; private set; }

    public string? UnitNumber { get; private set; }

    public bool IsCornerUnit { get; private set; }

    public decimal FloorAreaSqFt { get; private set; }

    public short Bedrooms { get; private set; }

    public short MasterBedrooms { get; private set; }

    public short Bathrooms { get; private set; }

    public short AttachedBathrooms { get; private set; }

    public short Balconies { get; private set; }

    public short? FurnishingTypeId { get; private set; }

    public short? ViewTypeId { get; private set; }

    public bool HasMaidRoom { get; private set; }

    public bool HasMaidBathroom { get; private set; }

    public bool HasLaundryArea { get; private set; }

    public bool HasStorageRoom { get; private set; }

    public bool HasWalkInCloset { get; private set; }

    public bool HasDriversRoom { get; private set; }

    public static ApartmentDetails Create(
        Guid propertyId,
        Guid? condominiumId,
        short? apartmentTypeId,
        int? floorNumber,
        string? unitNumber,
        bool isCornerUnit,
        decimal floorAreaSqFt,
        short bedrooms,
        short masterBedrooms,
        short bathrooms,
        short attachedBathrooms,
        short balconies,
        short? furnishingTypeId,
        short? viewTypeId,
        bool hasMaidRoom,
        bool hasMaidBathroom,
        bool hasLaundryArea,
        bool hasStorageRoom,
        bool hasWalkInCloset,
        bool hasDriversRoom)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException(
                "Property ID is required.",
                nameof(propertyId));

        if (floorAreaSqFt <= 0)
            throw new ArgumentException(
                "Floor area must be greater than zero.",
                nameof(floorAreaSqFt));

        if (bedrooms < 0)
            throw new ArgumentException(
                "Bedrooms cannot be negative.",
                nameof(bedrooms));

        if (masterBedrooms < 0)
            throw new ArgumentException(
                "Master bedrooms cannot be negative.",
                nameof(masterBedrooms));

        if (bathrooms < 0)
            throw new ArgumentException(
                "Bathrooms cannot be negative.",
                nameof(bathrooms));

        if (attachedBathrooms < 0)
            throw new ArgumentException(
                "Attached bathrooms cannot be negative.",
                nameof(attachedBathrooms));

        if (balconies < 0)
            throw new ArgumentException(
                "Balconies cannot be negative.",
                nameof(balconies));

        return new ApartmentDetails
        {
            PropertyId = propertyId,
            CondominiumId = condominiumId,
            ApartmentTypeId = apartmentTypeId,
            FloorNumber = floorNumber,
            UnitNumber = string.IsNullOrWhiteSpace(unitNumber)
                ? null
                : unitNumber.Trim(),
            IsCornerUnit = isCornerUnit,
            FloorAreaSqFt = floorAreaSqFt,
            Bedrooms = bedrooms,
            MasterBedrooms = masterBedrooms,
            Bathrooms = bathrooms,
            AttachedBathrooms = attachedBathrooms,
            Balconies = balconies,
            FurnishingTypeId = furnishingTypeId,
            ViewTypeId = viewTypeId,
            HasMaidRoom = hasMaidRoom,
            HasMaidBathroom = hasMaidBathroom,
            HasLaundryArea = hasLaundryArea,
            HasStorageRoom = hasStorageRoom,
            HasWalkInCloset = hasWalkInCloset,
            HasDriversRoom = hasDriversRoom
        };
    }

    public void Update(
        Guid? condominiumId,
        short? apartmentTypeId,
        int? floorNumber,
        string? unitNumber,
        bool isCornerUnit,
        decimal floorAreaSqFt,
        short bedrooms,
        short masterBedrooms,
        short bathrooms,
        short attachedBathrooms,
        short balconies,
        short? furnishingTypeId,
        short? viewTypeId,
        bool hasMaidRoom,
        bool hasMaidBathroom,
        bool hasLaundryArea,
        bool hasStorageRoom,
        bool hasWalkInCloset,
        bool hasDriversRoom)
    {
        if (floorAreaSqFt <= 0)
            throw new ArgumentException(
                "Floor area must be greater than zero.",
                nameof(floorAreaSqFt));

        if (bedrooms < 0 ||
            masterBedrooms < 0 ||
            bathrooms < 0 ||
            attachedBathrooms < 0 ||
            balconies < 0)
        {
            throw new ArgumentException(
                "Apartment counts cannot be negative.");
        }

        CondominiumId = condominiumId;
        ApartmentTypeId = apartmentTypeId;
        FloorNumber = floorNumber;
        UnitNumber = string.IsNullOrWhiteSpace(unitNumber)
            ? null
            : unitNumber.Trim();
        IsCornerUnit = isCornerUnit;
        FloorAreaSqFt = floorAreaSqFt;
        Bedrooms = bedrooms;
        MasterBedrooms = masterBedrooms;
        Bathrooms = bathrooms;
        AttachedBathrooms = attachedBathrooms;
        Balconies = balconies;
        FurnishingTypeId = furnishingTypeId;
        ViewTypeId = viewTypeId;
        HasMaidRoom = hasMaidRoom;
        HasMaidBathroom = hasMaidBathroom;
        HasLaundryArea = hasLaundryArea;
        HasStorageRoom = hasStorageRoom;
        HasWalkInCloset = hasWalkInCloset;
        HasDriversRoom = hasDriversRoom;
    }
}