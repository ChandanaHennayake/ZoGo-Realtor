namespace zogo.Application.DTOs.Admin;

public sealed class AdminPropertySummaryResponse
{
    public Guid PropertyId { get; set; }
    public string ReferenceNo { get; set; } = null!;
    public string Title { get; set; } = null!;
    public short PropertyTypeId { get; set; }
    public string PropertyTypeName { get; set; } = string.Empty;
    public short ListingTypeId { get; set; }
    public string ListingTypeName { get; set; } = string.Empty;
    public string DistrictName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public decimal AskingPrice { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public Guid OwnerUserId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public int PhotoCount { get; set; }
    public int DocumentCount { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class AdminPropertyReviewDetailsResponse
{
    public Guid PropertyId { get; set; }
    public string ReferenceNo { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public short PropertyTypeId { get; set; }
    public string PropertyTypeName { get; set; } = string.Empty;
    public short ListingTypeId { get; set; }
    public string ListingTypeName { get; set; } = string.Empty;
    public string ProvinceName { get; set; } = string.Empty;
    public string DistrictName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = null!;
    public string? AddressLine2 { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal AskingPrice { get; set; }
    public bool IsNegotiable { get; set; }
    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Owner info
    public Guid OwnerUserId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public string OwnerPhone { get; set; } = string.Empty;

    // Media & Docs
    public IReadOnlyList<AdminPropertyMediaItem> Media { get; set; } = [];
    public IReadOnlyList<AdminPropertyDocumentItem> Documents { get; set; } = [];

    // Specs
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public decimal? FloorAreaSqFt { get; set; }
    public decimal? LandAreaPerches { get; set; }
    public int? ParkingSpaces { get; set; }
    public IReadOnlyList<string> Amenities { get; set; } = [];
    public IReadOnlyList<string> Features { get; set; } = [];
}

public sealed class AdminPropertyMediaItem
{
    public Guid MediaId { get; set; }
    public string MediaUrl { get; set; } = null!;
    public bool IsCover { get; set; }
    public int DisplayOrder { get; set; }
}

public sealed class AdminPropertyDocumentItem
{
    public Guid DocumentId { get; set; }
    public string DocumentType { get; set; } = null!;
    public string DocumentName { get; set; } = null!;
    public string DocumentUrl { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
}

public sealed class RequestPropertyChangesRequest
{
    public string AdminNotes { get; set; } = null!;
    public List<string>? RequestedChanges { get; set; }
}

public sealed class RejectPropertyRequest
{
    public string RejectionReason { get; set; } = null!;
}

public sealed class AssignPropertyAgentRequest
{
    public Guid AgentUserId { get; set; }
}
