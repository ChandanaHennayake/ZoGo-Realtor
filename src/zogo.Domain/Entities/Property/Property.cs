namespace zogo.Domain.Entities.Property;

public class Property
{
    private Property()
    {
    }

    public Guid Id { get; private set; }

    public string ReferenceNo { get; private set; } = null!;

    public Guid OwnerUserId { get; private set; }

    public short PropertyTypeId { get; private set; }

    public short ListingTypeId { get; private set; }

    public string Title { get; private set; } = null!;

    public string? Description { get; private set; }

    public short DistrictId { get; private set; }

    public int? DivisionalSecretariatId { get; private set; }

    public int? GnDivisionId { get; private set; }

    public string AddressLine1 { get; private set; } = null!;

    public string? AddressLine2 { get; private set; }

    public string City { get; private set; } = null!;

    public string? PostalCode { get; private set; }

    public decimal? Latitude { get; private set; }

    public decimal? Longitude { get; private set; }

    public decimal AskingPrice { get; private set; }

    public bool IsNegotiable { get; private set; }

    public short Status { get; private set; }

    public DateTime? PublishedAt { get; private set; }

    public DateTime? SoldAt { get; private set; }

    public Guid CreatedBy { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Guid? UpdatedBy { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }


    public static Property CreateDraft(
        string referenceNo,
        Guid ownerUserId,
        short propertyTypeId,
        short listingTypeId,
        string title,
        string? description,
        short districtId,
        int? divisionalSecretariatId,
        int? gnDivisionId,
        string addressLine1,
        string? addressLine2,
        string city,
        string? postalCode,
        decimal? latitude,
        decimal? longitude,
        decimal askingPrice,
        bool isNegotiable,
        Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(referenceNo))
            throw new ArgumentException(
                "Reference number is required.",
                nameof(referenceNo));

        if (ownerUserId == Guid.Empty)
            throw new ArgumentException(
                "Owner user ID is required.",
                nameof(ownerUserId));

        if (propertyTypeId <= 0)
            throw new ArgumentException(
                "Property type is required.",
                nameof(propertyTypeId));

        if (listingTypeId <= 0)
            throw new ArgumentException(
                "Listing type is required.",
                nameof(listingTypeId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Property title is required.",
                nameof(title));

        if (districtId <= 0)
            throw new ArgumentException(
                "District is required.",
                nameof(districtId));

        if (string.IsNullOrWhiteSpace(addressLine1))
            throw new ArgumentException(
                "Address line 1 is required.",
                nameof(addressLine1));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException(
                "City is required.",
                nameof(city));

        if (askingPrice < 0)
            throw new ArgumentException(
                "Asking price cannot be negative.",
                nameof(askingPrice));

        if (createdBy == Guid.Empty)
            throw new ArgumentException(
                "Created by user ID is required.",
                nameof(createdBy));

        return new Property
        {
            Id = Guid.NewGuid(),

            ReferenceNo = referenceNo.Trim(),

            OwnerUserId = ownerUserId,

            PropertyTypeId = propertyTypeId,
            ListingTypeId = listingTypeId,

            Title = title.Trim(),

            Description = string.IsNullOrWhiteSpace(description)
                ? null
                : description.Trim(),

            DistrictId = districtId,
            DivisionalSecretariatId = divisionalSecretariatId,
            GnDivisionId = gnDivisionId,

            AddressLine1 = addressLine1.Trim(),

            AddressLine2 = string.IsNullOrWhiteSpace(addressLine2)
                ? null
                : addressLine2.Trim(),

            City = city.Trim(),

            PostalCode = string.IsNullOrWhiteSpace(postalCode)
                ? null
                : postalCode.Trim(),

            Latitude = latitude,
            Longitude = longitude,

            AskingPrice = askingPrice,
            IsNegotiable = isNegotiable,

            // Property status:
            // 1 = DRAFT
            Status = 1,

            PublishedAt = null,
            SoldAt = null,

            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow,

            UpdatedBy = null,
            UpdatedAt = null,
            DeletedAt = null
        };
    }


    public void Update(
        string title,
        string? description,
        short propertyTypeId,
        short listingTypeId,
        short districtId,
        int? divisionalSecretariatId,
        int? gnDivisionId,
        string addressLine1,
        string? addressLine2,
        string city,
        string? postalCode,
        decimal? latitude,
        decimal? longitude,
        decimal askingPrice,
        bool isNegotiable,
        Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(
                "Property title is required.",
                nameof(title));

        if (askingPrice < 0)
            throw new ArgumentException(
                "Asking price cannot be negative.",
                nameof(askingPrice));

        Title = title.Trim();

        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        PropertyTypeId = propertyTypeId;
        ListingTypeId = listingTypeId;

        DistrictId = districtId;
        DivisionalSecretariatId = divisionalSecretariatId;
        GnDivisionId = gnDivisionId;

        AddressLine1 = addressLine1.Trim();

        AddressLine2 = string.IsNullOrWhiteSpace(addressLine2)
            ? null
            : addressLine2.Trim();

        City = city.Trim();

        PostalCode = string.IsNullOrWhiteSpace(postalCode)
            ? null
            : postalCode.Trim();

        Latitude = latitude;
        Longitude = longitude;

        AskingPrice = askingPrice;
        IsNegotiable = isNegotiable;

        UpdatedBy = updatedBy;
        UpdatedAt = DateTime.UtcNow;
    }


    public void ChangeStatus(
        short status,
        Guid changedBy)
    {
        if (status <= 0)
            throw new ArgumentException(
                "Property status is required.",
                nameof(status));

        Status = status;

        UpdatedBy = changedBy;
        UpdatedAt = DateTime.UtcNow;
    }


    public void SoftDelete(Guid deletedBy)
    {
        DeletedAt = DateTime.UtcNow;
        UpdatedBy = deletedBy;
        UpdatedAt = DateTime.UtcNow;
    }
}