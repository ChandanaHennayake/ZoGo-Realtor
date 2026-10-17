namespace zogo.Application.DTOs.Visits;

public sealed class CreateVisitRequest
{
    public Guid PropertyId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? TimeSlot { get; set; }
    public string? BuyerNotes { get; set; }
}

public sealed class ConfirmVisitRequest
{
    public Guid? AssignedAgentId { get; set; }
    public string? Notes { get; set; }
}

public sealed class CompleteVisitRequest
{
    public string? Notes { get; set; }
}

public sealed class CancelVisitRequest
{
    public string? Reason { get; set; }
}

public sealed class VisitResponse
{
    public Guid VisitId { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyTitle { get; set; } = null!;
    public string PropertyReferenceNo { get; set; } = null!;
    public string DistrictName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;

    public Guid BuyerId { get; set; }
    public string BuyerName { get; set; } = null!;
    public string BuyerEmail { get; set; } = string.Empty;
    public string BuyerPhone { get; set; } = string.Empty;

    public Guid? AssignedAgentId { get; set; }
    public string? AssignedAgentName { get; set; }

    public DateTime ScheduledAt { get; set; }
    public string? TimeSlot { get; set; }
    public string? BuyerNotes { get; set; }
    public string? AgentNotes { get; set; }

    public short Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
