namespace zogo.Application.DTOs.Common;

public sealed class GramaNiladhariDivisionResponse
{
    public int Id { get; set; }
    public int DivisionalSecretariatId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
