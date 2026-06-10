using MediaService.Domain.Enums;
using MediaService.Domain.Models;

namespace MediaService.Application.Commands;

public sealed record UploadAttachmentBatchCommand(
    Guid TenantId,
    string Category,
    List<IFormFile> Files,
    MediaAccessLevel? AccessLevel,
    OwnerReference Owner,
    string? OwnerId);