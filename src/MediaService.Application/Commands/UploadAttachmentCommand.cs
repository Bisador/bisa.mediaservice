using MediaService.Domain.Enums;
using MediaService.Domain.Models;

namespace MediaService.Application.Commands;

public sealed record UploadAttachmentCommand(
    Guid TenantId,
    string Category,
    IFormFile File,
    MediaAccessLevel? AccessLevel,
    OwnerReference Owner,
    string? OwnerId);