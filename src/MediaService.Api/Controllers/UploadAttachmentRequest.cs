using MediaService.Domain.Enums;

namespace MediaService.Api.Controllers;

public sealed record UploadAttachmentRequest(
    string Category,
    IFormFile File,
    MediaAccessLevel? AccessLevel,
    string OwnerType,
    string OwnerId);