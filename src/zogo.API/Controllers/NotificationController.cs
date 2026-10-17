using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Common;
using zogo.Application.DTOs.Notification;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationController : ApiControllerBase
{
    private readonly IUserNotificationService _notificationService;

    public NotificationController(IUserNotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int limit = 50,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        var result = await _notificationService.GetUserNotificationsAsync(userId.Value, limit, cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<UserNotificationResponse>>.Ok(result));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        var result = await _notificationService.GetUnreadCountAsync(userId.Value, cancellationToken);
        return Ok(ApiResponse<UnreadCountResponse>.Ok(result));
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkAsRead(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        var success = await _notificationService.MarkAsReadAsync(id, userId.Value, cancellationToken);
        if (!success)
            return NotFound(ApiResponse.Fail("Notification not found or access denied."));

        return Ok(ApiResponse.Ok("Notification marked as read."));
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized();

        await _notificationService.MarkAllAsReadAsync(userId.Value, cancellationToken);
        return Ok(ApiResponse.Ok("All notifications marked as read."));
    }

    [HttpPost("send")]
    [Authorize(Roles = "ADM,Admin")]
    public async Task<IActionResult> SendNotification(
        [FromBody] SendNotificationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _notificationService.SendNotificationAsync(request, cancellationToken);
        return Ok(ApiResponse<UserNotificationResponse>.Ok(result, "Notification dispatched."));
    }

    private Guid? GetUserId()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        return Guid.TryParse(userIdClaim, out var userId)
            ? userId
            : null;
    }
}
