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
        var tenantId = User.GetTenantId();

        var command = new UploadPersonalMediaCommand(tenantId, request.File, request.AccessLevel, ownerId);

        var result = await service.UploadPersonalAsync(command, cancellationToken);

        return CreatedAtAction(
            nameof(Download),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPost("attachments")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult<MediaResponse>> UploadAttachment(
        [FromForm] UploadAttachmentRequest request,
        CancellationToken cancellationToken)
    {
        var ownerId = User.GetUserId();
        var tenantId = User.GetTenantId();

        var command = new UploadAttachmentCommand(
            tenantId,
            request.Category,
            request.File,
            request.AccessLevel,
            new OwnerReference(request.OwnerType, request.OwnerId),
            ownerId);

        var result = await service.UploadAttachmentAsync(command, cancellationToken);

        if (result.IsFailure)
            return result.ToActionResult(this);

        return CreatedAtAction(
            nameof(Download),
            new { id = result.Value.Id },
            result.Value);
    }
    
    [HttpPost("attachments/batch")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult<List<MediaResponse>>> UploadAttachments(
        [FromForm] UploadAttachmentBatchRequest request,
        CancellationToken cancellationToken)
    {
        var ownerId = User.GetUserId();
        var tenantId = User.GetTenantId();

        var command = new UploadAttachmentBatchCommand(
            tenantId,
            request.Category,
            request.File,
            request.AccessLevel,
            new OwnerReference(request.OwnerType, request.OwnerId),
            ownerId);

        var result = await service.UploadAttachmentBatchAsync(command, cancellationToken);
 
        return result.ToActionResult(this); 
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadPublic(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.DownloadPublicAsync(id, cancellationToken);
        return File(result.Value.Stream, result.Value.ContentType, result.Value.FileName);
    }

    [HttpGet("internal/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();
        var result = await service.DownloadInternalAsync(id, tenantId, cancellationToken);
        return File(result.Value.Stream, result.Value.ContentType, result.Value.FileName);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();

        await service.DeleteAsync(tenantId, id, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/links")]
    public async Task<IActionResult> AddLink(
        Guid id,
        AddMediaLinkRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();

        var command = new LinkMediaCommand(
            tenantId,
            id,
            new OwnerReference(request.OwnerType, request.OwnerId));

        var result = await service.LinkAsync(command, cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpDelete("{id:guid}/links/{linkId:guid}")]
    public async Task<IActionResult> RemoveLink(
        Guid id,
        Guid linkId,
        CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();

        var result = await service.RemoveLinkAsync(new RemoveMediaLinkCommand(tenantId, id, linkId), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpDelete("{id:guid}/links")]
    public async Task<IActionResult> RemoveLink(
        Guid id,
        RemoveLinkRequest request,
        CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();

        var result = await service.RemoveLinkByOwnerAsync(
            new RemoveMediaLinkByOwnerCommand(tenantId, id, request.OwnerType, request.OwnerId), cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPost("validate")]
    public async Task<ActionResult<MediaValidationResponse>>
        Validate(
            ValidateMediaRequest request,
            CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();

        var result = await service.ValidateAsync(
            new ValidateMediaCommand(tenantId, request.MediaIds),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpGet("{id:guid}/metadata")]
    public async Task<ActionResult<MediaMetadataResponse>> Metadata(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();

        var result = await service.MetadataAsync(new MetadataMediaCommand(tenantId, id), cancellationToken);
        return result.ToActionResult(this);
    }
    
    [HttpGet("metadata")]
    public async Task<ActionResult<List<MediaMetadataResponse>>> MetadataBatch(
        List<Guid> id,
        CancellationToken cancellationToken)
    {
        var tenantId = User.GetTenantId();

        var result = await service.MetadataBatchAsync(new MetadataBatchMediaCommand(tenantId, id), cancellationToken);
        return result.ToActionResult(this);
    }
}