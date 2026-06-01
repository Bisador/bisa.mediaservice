using MediaService.Domain.Models;

namespace MediaService.Application.Commands;

public sealed record UploadAttachmentCommand(
    Guid TenantId,
    string Category,
    IFormFile File,
    OwnerReference Owner,
    string? OwnerId);