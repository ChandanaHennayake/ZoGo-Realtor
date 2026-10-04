using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.DTOs.Property.PropertyDocument;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/properties/{propertyId:guid}/documents")]
[Authorize(Roles = "SEL")]
public class PropertyDocumentController : ControllerBase
{
    private readonly IPropertyDocumentService _propertyDocumentService;
    private readonly IFileStorageService _fileStorageService;

    public PropertyDocumentController(
        IPropertyDocumentService propertyDocumentService,
        IFileStorageService fileStorageService)
    {
        _propertyDocumentService = propertyDocumentService;
        _fileStorageService = fileStorageService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(
        Guid propertyId,
        [FromForm] UploadPropertyDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        if (request.File is null || request.File.Length == 0)
            return BadRequest("File is required.");

        if (request.DocumentTypeId <= 0)
            return BadRequest("Document type ID is required.");

        var extension = Path.GetExtension(request.File.FileName);
        var documentId = Guid.NewGuid();
        var storageKey = $"properties/{propertyId}/documents/{documentId:N}{extension}";

        try
        {
            await using var stream = request.File.OpenReadStream();

            await _fileStorageService.UploadAsync(
                stream,
                storageKey,
                request.File.ContentType,
                cancellationToken);

            var createRequest = new CreatePropertyDocumentRequest
            {
                DocumentTypeId = request.DocumentTypeId,
                StorageKey = storageKey,
                OriginalFileName = request.File.FileName,
                MimeType = request.File.ContentType,
                FileSizeBytes = request.File.Length
            };

            var result = await _propertyDocumentService.AddAsync(
                userId,
                propertyId,
                createRequest,
                cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { propertyId, documentId = result.Id },
                result);
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
                // Preserve original exception
            }

            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByPropertyId(
        Guid propertyId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _propertyDocumentService.GetByPropertyIdAsync(
            userId,
            propertyId,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{documentId:guid}")]
    public async Task<IActionResult> GetById(
        Guid propertyId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _propertyDocumentService.GetByIdAsync(
            userId,
            propertyId,
            documentId,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{documentId:guid}/download")]
    public async Task<IActionResult> Download(
        Guid propertyId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _propertyDocumentService.DownloadAsync(
            userId,
            propertyId,
            documentId,
            cancellationToken);

        return File(
            result.Stream,
            result.MimeType,
            result.FileName);
    }

    [HttpDelete("{documentId:guid}")]
    public async Task<IActionResult> Delete(
        Guid propertyId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        await _propertyDocumentService.DeleteAsync(
            userId,
            propertyId,
            documentId,
            cancellationToken);

        return NoContent();
    }

    [HttpPatch("{documentId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid propertyId,
        Guid documentId,
        [FromBody] UpdatePropertyDocumentStatusRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _propertyDocumentService.UpdateStatusAsync(
            userId,
            propertyId,
            documentId,
            request,
            cancellationToken);

        return Ok(result);
    }

    private Guid GetUserId()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user identity.");

        return userId;
    }
}
