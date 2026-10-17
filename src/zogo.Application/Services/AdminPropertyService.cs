using zogo.Application.DTOs.Admin;
using zogo.Application.Exceptions;
using zogo.Application.Interfaces.Repositories;
using zogo.Application.Interfaces.Services;
using zogo.Domain.Entities.Notification;

namespace zogo.Application.Services;

public sealed class AdminPropertyService : IAdminPropertyService
{
    private readonly IAdminPropertyRepository _adminPropertyRepository;
    private readonly IUserNotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminPropertyService(
        IAdminPropertyRepository adminPropertyRepository,
        IUserNotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _adminPropertyRepository = adminPropertyRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<AdminPropertySummaryResponse>> GetPropertiesByStatusAsync(
        short? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var page = pageNumber < 1 ? 1 : pageNumber;
        var size = pageSize < 1 ? 20 : pageSize;

        return await _adminPropertyRepository.GetPropertiesByStatusAsync(
            status,
            page,
            size,
            cancellationToken);
    }

    public async Task<AdminPropertyReviewDetailsResponse?> GetPropertyReviewDetailsAsync(
        Guid propertyId,
        CancellationToken cancellationToken = default)
    {
        if (propertyId == Guid.Empty)
            throw new ArgumentException("Property ID is required.", nameof(propertyId));

        return await _adminPropertyRepository.GetPropertyReviewDetailsAsync(
            propertyId,
            cancellationToken);
    }

    public async Task<bool> StartReviewAsync(
        Guid propertyId,
        Guid adminUserId,
        CancellationToken cancellationToken = default)
    {
        var property = await _adminPropertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null || property.DeletedAt.HasValue)
            throw new NotFoundException("Property", propertyId);

        // Status 3 = UNDER_REVIEW
        property.ChangeStatus(3, adminUserId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ApprovePropertyAsync(
        Guid propertyId,
        Guid adminUserId,
        CancellationToken cancellationToken = default)
    {
        var property = await _adminPropertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null || property.DeletedAt.HasValue)
            throw new NotFoundException("Property", propertyId);

        // Status 2 = PUBLISHED (Approved and immediately published for discovery)
        property.ChangeStatus(2, adminUserId);

        var notification = UserNotification.Create(
            property.OwnerUserId,
            "PROPERTY_APPROVED",
            "Listing Approved! 🎉",
            $"Your property '{property.Title}' (Ref: {property.ReferenceNo}) has been approved and is now live on ZoGo Realtor.",
            property.Id.ToString(),
            "Property");
        await _notificationRepository.AddAsync(notification, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RequestChangesAsync(
        Guid propertyId,
        RequestPropertyChangesRequest request,
        Guid adminUserId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.AdminNotes))
            throw new ValidationException("AdminNotes", "Review comments/notes are required when requesting changes.");

        var property = await _adminPropertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null || property.DeletedAt.HasValue)
            throw new NotFoundException("Property", propertyId);

        // Status 9 = CHANGES_REQUESTED
        property.ChangeStatus(9, adminUserId);

        var notification = UserNotification.Create(
            property.OwnerUserId,
            "CHANGES_REQUESTED",
            "Changes Requested on Listing",
            $"Admin reviewed '{property.Title}' (Ref: {property.ReferenceNo}) and requested updates: {request.AdminNotes.Trim()}",
            property.Id.ToString(),
            "Property");
        await _notificationRepository.AddAsync(notification, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RejectPropertyAsync(
        Guid propertyId,
        RejectPropertyRequest request,
        Guid adminUserId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.RejectionReason))
            throw new ValidationException("RejectionReason", "Rejection reason is required.");

        var property = await _adminPropertyRepository.GetByIdAsync(propertyId, cancellationToken);
        if (property is null || property.DeletedAt.HasValue)
            throw new NotFoundException("Property", propertyId);

        // Status 10 = REJECTED
        property.ChangeStatus(10, adminUserId);

        var notification = UserNotification.Create(
            property.OwnerUserId,
            "PROPERTY_REJECTED",
            "Listing Declined",
            $"Your listing '{property.Title}' (Ref: {property.ReferenceNo}) was declined: {request.RejectionReason.Trim()}",
            property.Id.ToString(),
            "Property");
        await _notificationRepository.AddAsync(notification, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<AdminDashboardStatsResponse> GetDashboardStatsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _adminPropertyRepository.GetDashboardStatsAsync(cancellationToken);
    }
}
