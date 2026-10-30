using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.DTOs.Property.PropertyMedia;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/properties/{propertyId:guid}/media")]
[Authorize]
public class PropertyMediaController : ControllerBase
{
    private readonly IPropertyMediaService _propertyMediaService;
    private readonly IFileStorageService _fileStorageService;

    public PropertyMediaController(
        IPropertyMediaService propertyMediaService,
        IFileStorageService fileStorageService)
    {
        _propertyMediaService = propertyMediaService;
        _fileStorageService = fileStorageService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(
        Guid propertyId,
        [FromForm] UploadPropertyMediaRequest request,
        CancellationToken cancellationToken)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        if (request.File is null || request.File.Length == 0)
            return BadRequest("File is required.");

        if (request.File.Length <= 0)
            return BadRequest("File cannot be empty.");

        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        var contentType = request.File.ContentType;
        if (string.IsNullOrWhiteSpace(contentType) || contentType == "application/octet-stream")
        {
            contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                ".mp4" => "video/mp4",
                ".mov" => "video/quicktime",
                _ => "image/jpeg"
            };
        }

        var mediaType = request.MediaType <= 0 ? (short)1 : request.MediaType;
        var mediaId = Guid.NewGuid();
        var storageKey = $"properties/{propertyId}/{mediaId:N}{extension}";

        try
        {
            await using var stream = request.File.OpenReadStream();

            await _fileStorageService.UploadAsync(
                stream,
                storageKey,
                contentType,
                cancellationToken);

            var createRequest =
                new CreatePropertyMediaRequest
                {
                    MediaType = mediaType,
                    StorageKey = storageKey,
                    OriginalFileName = request.File.FileName,
                    MimeType = contentType,
                    FileSizeBytes = request.File.Length,
                    DisplayOrder = request.DisplayOrder,
                    IsCover = request.IsCover
                };

            var result =
                await _propertyMediaService.AddAsync(
                    userId,
                    propertyId,
                    createRequest,
                    cancellationToken);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch
        {
            try
            {
                await _fileStorageService.DeleteAsync(
                    storageKey,
                    cancellationToken);
            }
            catch
            {
            }

            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByPropertyId(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var result =
            await _propertyMediaService.GetByPropertyIdAsync(
                userId,
                propertyId,
                cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{mediaId:guid}")]
    public async Task<IActionResult> Delete(
        Guid propertyId,
        Guid mediaId,
        CancellationToken cancellationToken)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        await _propertyMediaService.DeleteAsync(
            userId,
            propertyId,
            mediaId,
            cancellationToken);

        return NoContent();
    }


    [HttpGet("{mediaId:guid}/download")]
    [AllowAnonymous]
    public async Task<IActionResult> Download(
        Guid propertyId,
        Guid mediaId,
        CancellationToken cancellationToken)
    {
        var result = await _propertyMediaService.DownloadAsync(
            null,
            propertyId,
            mediaId,
            cancellationToken);

        return File(
            result.Stream,
            result.MimeType,
            result.FileName);
    }

}