using MediaService.Domain.Models;

namespace MediaService.Application.Commands;

public sealed record UploadAttachmentCommand(
    Guid TenantId,
    IFormFile File,
    OwnerReference Owner,
    string? OwnerId);