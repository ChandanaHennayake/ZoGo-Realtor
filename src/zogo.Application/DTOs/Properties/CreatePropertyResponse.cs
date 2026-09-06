namespace zogo.Application.DTOs.Properties;

public sealed class CreatePropertyResponse
{
    public Guid PropertyId { get; set; }

    public string ReferenceNo { get; set; } = null!;

    public Guid OwnerUserId { get; set; }

    public short Status { get; set; }

    public DateTime CreatedAt { get; set; }
}