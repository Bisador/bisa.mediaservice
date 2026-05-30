using MediaService.Api.Controllers.Requests;
using MediaService.Api.Extensions;
using MediaService.Application.Abstractions;
using MediaService.Application.Commands;
using MediaService.Application.DTOs;
using MediaService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediaService.Api.Controllers;

[ApiController]
[Route("api/media")]
public sealed class MediaController(IMediaAppService service) : ControllerBase
{
    [HttpPost("personal")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult<MediaResponse>> UploadPersonal(
        [FromForm] UploadRequest request,
        CancellationToken cancellationToken)
    {
        var ownerId = User.GetUserId();

        var command = new UploadPersonalMediaCommand(request.File, ownerId);

        var result = await service.UploadPersonalAsync(command, cancellationToken);

        return result.Match(Ok, Problem);
    }

    [HttpPost("attachments")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult<MediaResponse>> UploadAttachment(
        [FromForm] UploadAttachmentRequest request,
        CancellationToken cancellationToken)
    {
        var ownerId = User.GetUserId(); 

        var command = new UploadAttachmentCommand(
            request.File,
            new OwnerReference(request.OwnerType, request.OwnerEntityId),
            ownerId);

        var result = await service.UploadAttachmentAsync(command, cancellationToken);

        return result.Match(Ok, Problem);
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

    [HttpPost("{id:guid}/links")]
    public async Task<IActionResult> AddLink(
        Guid id,
        AddMediaLinkRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LinkMediaCommand(
            id,
            new OwnerReference(request.OwnerType, request.OwnerId));

        var result = await service.LinkAsync(command, cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpDelete("{id:guid}/links/{linkId:guid}")]
    public async Task<IActionResult> RemoveLink(
        Guid id,
        Guid linkId,
        CancellationToken cancellationToken)
    {
        var result = await service.RemoveLinkAsync(new RemoveMediaLinkCommand(id, linkId), cancellationToken);

        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("validate")]
    public async Task<ActionResult<MediaValidationResponse>>
        Validate(
            ValidateMediaRequest request,
            CancellationToken cancellationToken)
    {
        var result = await service.ValidateAsync(new ValidateMediaCommand(request.MediaIds), cancellationToken);

        return result.Match(Ok, Problem);
    }
}