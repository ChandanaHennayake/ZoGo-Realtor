namespace zogo.Application.DTOs.Properties;

public sealed class CreateApartmentDetailsRequest
{
    public Guid? CondominiumId { get; set; }

    public short? ApartmentTypeId { get; set; }

    public int? FloorNumber { get; set; }

    public string? UnitNumber { get; set; }

    public bool IsCornerUnit { get; set; }

    public decimal FloorAreaSqFt { get; set; }

    public short Bedrooms { get; set; }

    public short MasterBedrooms { get; set; }

    public short Bathrooms { get; set; }

    public short AttachedBathrooms { get; set; }

    public short Balconies { get; set; }

    public short? FurnishingTypeId { get; set; }

    public short? ViewTypeId { get; set; }

    public bool HasMaidRoom { get; set; }

    public bool HasMaidBathroom { get; set; }

    public bool HasLaundryArea { get; set; }

    public bool HasStorageRoom { get; set; }

    public bool HasWalkInCloset { get; set; }

    public bool HasDriversRoom { get; set; }
}