using System.Security.Claims;
using Api.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Storage;

namespace Api.Controllers;

//Luka Kanjir
[Authorize]
[ApiController]
public sealed class ImagesController(IImageService imageService, ILogger<ImagesController> logger) : ControllerBase
{
    [HttpPost(ApiEndpoints.Images.UploadEvent)]
    [RequestSizeLimit(10_485_760)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadEvent([FromForm] IFormFile? file, [FromForm] int eventId,
        CancellationToken ct = default)
    {
        if (file is null) return BadRequest("Slika je obavezna");

        await using var stream = file.OpenReadStream();
        var upload = new ImageUpload(stream, file.ContentType ?? string.Empty, file.Length);

        try
        {
            var path = await imageService.UploadEventAsync(eventId, upload, ct);
            return Ok(new { path });
        }
        catch (ImageException ex)
        {
            return MapImageError(ex);
        }
    }

    [HttpGet(ApiEndpoints.Images.GetEvent)]
    public async Task<IActionResult> GetEvent(int eventId, CancellationToken ct = default)
    {
        try
        {
            var result = await imageService.GetEventImageAsync(eventId, ct);
            return File(result.Content, result.ContentType, enableRangeProcessing: true);
        }
        catch (ImageException ex)
        {
            return MapImageError(ex);
        }
    }

    [HttpPost(ApiEndpoints.Images.UploadFault)]
    [RequestSizeLimit(10_485_760)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFault([FromForm] IFormFile? file, [FromForm] int faultId,
        CancellationToken ct = default)
    {
        if (file is null) return BadRequest("Slika je obavezna");

        var sub = GetSub();
        if (string.IsNullOrWhiteSpace(sub)) return Unauthorized();

        await using var stream = file.OpenReadStream();
        var upload = new ImageUpload(stream, file.ContentType ?? string.Empty, file.Length);

        try
        {
            var path = await imageService.UploadFaultAsync(faultId, upload, sub, ct);
            return Ok(new { path });
        }
        catch (ImageException ex)
        {
            return MapImageError(ex);
        }
    }

    [HttpGet(ApiEndpoints.Images.GetFault)]
    public async Task<IActionResult> GetFault(int faultId, CancellationToken ct = default)
    {
        var sub = GetSub();
        if (string.IsNullOrWhiteSpace(sub)) return Unauthorized();

        try
        {
            var result = await imageService.GetFaultImageAsync(faultId, sub, ct);
            return File(result.Content, result.ContentType, enableRangeProcessing: true);
        }
        catch (ImageException ex)
        {
            return MapImageError(ex);
        }
    }

    [HttpPost(ApiEndpoints.Images.UploadProfile)]
    [RequestSizeLimit(10_485_760)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProfile([FromForm] IFormFile? file, CancellationToken ct = default)
    {
        if (file is null) return BadRequest("Slika je obavezna");

        var sub = GetSub();
        if (string.IsNullOrWhiteSpace(sub)) return Unauthorized();

        await using var stream = file.OpenReadStream();
        var upload = new ImageUpload(stream, file.ContentType ?? string.Empty, file.Length);

        try
        {
            var path = await imageService.UploadProfileAsync(upload, sub, ct);
            return Ok(new { path });
        }
        catch (ImageException ex)
        {
            return MapImageError(ex);
        }
    }

    [HttpGet(ApiEndpoints.Images.GetProfile)]
    public async Task<IActionResult> GetProfile(int userId, CancellationToken ct = default)
    {
        try
        {
            var result = await imageService.GetProfileImageAsync(userId, ct);
            return File(result.Content, result.ContentType, enableRangeProcessing: true);
        }
        catch (ImageException ex)
        {
            return MapImageError(ex);
        }
    }

    private string? GetSub()
    {
        var sub = User.FindFirst("sub")?.Value;
        var nameId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        logger.LogDebug("GetSub daje sub = {sub}, nameId = {nameId}", sub, nameId);

        return sub ?? nameId;
    }

    private IActionResult MapImageError(ImageException ex) => ex.Code switch
    {
        ImageErrorCode.NotFound => NotFound(ex.Message),
        ImageErrorCode.Unauthorized => Unauthorized(ex.Message),
        ImageErrorCode.Forbidden => Forbid(),
        ImageErrorCode.Invalid => BadRequest(ex.Message),
        ImageErrorCode.TooLarge => StatusCode(StatusCodes.Status413PayloadTooLarge, ex.Message),
        ImageErrorCode.UnsupportedType => BadRequest(ex.Message),
        _ => StatusCode(StatusCodes.Status500InternalServerError, ex.Message)
    };
}