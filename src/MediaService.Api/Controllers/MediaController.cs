using MediaService.Api.Controllers.Requests;
using MediaService.Application.Abstractions;
using MediaService.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Api.Controllers;

[ApiController]
[Route("api/media")]
public sealed class MediaController(IMediaAppService service) : ControllerBase
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult<MediaResponse>> Upload(
        [FromForm] UploadRequest request,
        CancellationToken cancellationToken)
    {
        var ownerId = User.FindFirst("sub")?.Value;
        var result = await service.UploadAsync(request.File, ownerId, cancellationToken);
        if (result.IsFailure)
            return new NotFoundResult();
        return CreatedAtAction(nameof(Download), new { id = result.Value.Id }, result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.DownloadAsync(id, cancellationToken);
        return File(result.Value.Stream, result.Value.ContentType, result.Value.FileName);
    }


    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}