using Api.Configuration;
using Microsoft.AspNetCore.Mvc;
using Server.Application.Storage;

namespace Api.Controllers;

[ApiController]
public sealed class ImagesController(IImageService imageService) : ControllerBase
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
            return Ok(new {  path });
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