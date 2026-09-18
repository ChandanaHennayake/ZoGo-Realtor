using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using zogo.Application.Interfaces.Services;

namespace zogo.API.Controllers;

[ApiController]
[Route("api/v1/test-storage")]

public class StorageTestController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;

    public StorageTestController(
        IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest("File is required.");

        var extension = Path.GetExtension(file.FileName);

        var storageKey =
            $"test/{Guid.NewGuid():N}{extension}";

        await using var stream = file.OpenReadStream();

        var result = await _fileStorageService.UploadAsync(
            stream,
            storageKey,
            file.ContentType,
            cancellationToken);

        return Ok(new
        {
            message = "File uploaded successfully.",
            storageKey = result
        });
    }
}