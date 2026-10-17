namespace zogo.Application.DTOs.Buyer;

public sealed class BuyerPropertyListQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public short? PropertyTypeId { get; set; }
    public short? ListingTypeId { get; set; }
    public short? DistrictId { get; set; }
    public int? CityId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
