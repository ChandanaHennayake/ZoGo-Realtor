using zogo.Application.DTOs.Common;

namespace zogo.Application.Interfaces.Services;

public interface ICommonService
{
    Task<IReadOnlyList<ProvinceResponse>> GetProvincesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DistrictResponse>> GetDistrictsAsync(short? provinceId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CityResponse>> GetCitiesByDistrictAsync(short districtId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DivisionalSecretariatResponse>> GetDivisionalSecretariatsAsync(short districtId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GramaNiladhariDivisionResponse>> GetGramaNiladhariDivisionsAsync(int divisionalSecretariatId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PropertyStatusResponse>> GetPropertyStatusesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PropertyTypeResponse>> GetPropertyTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ListingTypeResponse>> GetListingTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ApartmentTypeResponse>> GetApartmentTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FurnishingTypeResponse>> GetFurnishingTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ViewTypeResponse>> GetViewTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CondominiumResponse>> GetCondominiumsAsync(short? districtId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AmenityMasterResponse>> GetAmenitiesAsync(string? category = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeatureMasterResponse>> GetFeaturesAsync(string? category = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DocumentTypeResponse>> GetDocumentTypesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OwnershipTypeResponse>> GetOwnershipTypesAsync(CancellationToken cancellationToken = default);
}

