using zogo.Application.DTOs.Properties;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Buyer;
using zogo.Domain.Entities.Master;

namespace zogo.Application.Services;

public sealed class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FavoriteService(
        IFavoriteRepository favoriteRepository,
        IPropertyRepository propertyRepository,
        IUnitOfWork unitOfWork)
    {
        _favoriteRepository = favoriteRepository;
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> AddFavoriteAsync(
        Guid userId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        var property = await _propertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null)
            throw new KeyNotFoundException("Property not found.");

        if (property.Status != 2)
            throw new InvalidOperationException("Property is not available for buyer viewing.");

        var existing = await _favoriteRepository.GetAsync(userId, propertyId, cancellationToken);
        if (existing is not null)
            return false;

        var favorite = Favorite.Create(userId, propertyId);
        await _favoriteRepository.AddAsync(favorite, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> RemoveFavoriteAsync(
        Guid userId,
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        var existing = await _favoriteRepository.GetAsync(userId, propertyId, cancellationToken);
        if (existing is null)
            return false;

        await _favoriteRepository.DeleteAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<IReadOnlyList<GetPropertyResponse>> GetFavoritesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));

        var properties = await _favoriteRepository.GetFavoritePropertiesByUserIdAsync(userId, cancellationToken);

        return properties.Select(MapToResponse).ToList();
    }

    private static GetPropertyResponse MapToResponse(Property property)
    {
        return new GetPropertyResponse
        {
            PropertyId = property.Id,
            ReferenceNo = property.ReferenceNo,
            OwnerUserId = property.OwnerUserId,
            PropertyTypeId = property.PropertyTypeId,
            ListingTypeId = property.ListingTypeId,
            Title = property.Title,
            Description = property.Description,
            ProvinceId = property.ProvinceId,
            DistrictId = property.DistrictId,
            DivisionalSecretariatId = property.DivisionalSecretariatId,
            GnDivisionId = property.GnDivisionId,
            CityId = property.CityId,
            AddressLine1 = property.AddressLine1,
            AddressLine2 = property.AddressLine2,
            PostalCode = property.PostalCode,
            Latitude = property.Latitude,
            Longitude = property.Longitude,
            AskingPrice = property.AskingPrice,
            IsNegotiable = property.IsNegotiable,
            Status = property.Status,
            StatusName = ResolveStatusName(property.Status),
            PublishedAt = property.PublishedAt,
            SoldAt = property.SoldAt,
            CreatedBy = property.CreatedBy,
            CreatedAt = property.CreatedAt,
            UpdatedBy = property.UpdatedBy,
            UpdatedAt = property.UpdatedAt
        };
    }

    private static string ResolveStatusName(short status) =>
        status switch
        {
            1 => "Draft",
            2 => "Published",
            3 => "Under Review",
            4 => "Pending Approval",
            5 => "Sold",
            6 => "Rented",
            7 => "Suspended",
            8 => "Inactive",
            _ => "Unknown"
        };
}
