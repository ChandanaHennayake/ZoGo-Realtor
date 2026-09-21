using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.DTOs.Property.PropertyMedia;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/properties/{propertyId:guid}/media")]
[Authorize(Roles = "SEL")]
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

        var extension =
            Path.GetExtension(request.File.FileName);

        var mediaId = Guid.NewGuid();

        var storageKey =
            $"properties/{propertyId}/{mediaId:N}{extension}";

        try
        {
            await using var stream =
                request.File.OpenReadStream();

            await _fileStorageService.UploadAsync(
                stream,
                storageKey,
                request.File.ContentType,
                cancellationToken);

            var createRequest =
                new CreatePropertyMediaRequest
                {
                    MediaType = request.MediaType,
                    StorageKey = storageKey,
                    OriginalFileName = request.File.FileName,
                    MimeType = request.File.ContentType,
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
        catch
        {
            // If database saving fails after the R2 upload,
            // remove the uploaded object to avoid an orphan file.
            try
            {
                await _fileStorageService.DeleteAsync(
                    storageKey,
                    cancellationToken);
            }
            catch
            {
                // Do not hide the original exception.
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
public async Task<IActionResult> Download(
    Guid propertyId,
    Guid mediaId,
    CancellationToken cancellationToken)
{
    var userIdClaim =
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub");

    if (!Guid.TryParse(userIdClaim, out var userId))
        return Unauthorized();

    var result = await _propertyMediaService.DownloadAsync(
        userId,
        propertyId,
        mediaId,
        cancellationToken);

    return File(
        result.Stream,
        result.MimeType,
        result.FileName);
}

}