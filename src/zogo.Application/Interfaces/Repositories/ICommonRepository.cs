using zogo.Domain.Entities.Master;

namespace zogo.Application.Interfaces.Repositories;

public interface ICommonRepository
{
    Task<IReadOnlyList<Province>> GetProvincesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<District>> GetDistrictsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<District>> GetDistrictsByProvinceAsync(
        short provinceId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<City>> GetCitiesByDistrictAsync(
        short districtId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DivisionalSecretariat>>
        GetDivisionalSecretariatsByDistrictAsync(
            short districtId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GramaNiladhariDivision>>
        GetGnDivisionsByDsAsync(
            int divisionalSecretariatId,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PropertyType>> GetPropertyTypesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ListingType>> GetListingTypesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApartmentType>> GetApartmentTypesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FurnishingType>> GetFurnishingTypesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ViewType>> GetViewTypesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Condominium>> GetCondominiumsAsync(
        short? districtId = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Amenity>> GetAmenitiesAsync(
        string? category = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Feature>> GetFeaturesAsync(
        string? category = null,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DocumentType>> GetDocumentTypesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OwnershipType>> GetOwnershipTypesAsync(
        CancellationToken cancellationToken = default);
}