namespace zogo.Application.DTOs.Buyer;

public sealed class SetPropertyInterestRequest
{
    public Guid? PropertyId { get; set; }
    public short Status { get; set; }
}
