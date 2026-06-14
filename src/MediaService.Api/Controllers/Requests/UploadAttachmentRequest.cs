using MediaService.Domain.Enums;

namespace MediaService.Api.Controllers.Requests;

public sealed record UploadAttachmentRequest(
    string Category,
    IFormFile File,
    MediaAccessLevel? AccessLevel);
     
     
public sealed record UploadAttachmentWithLinkRequest(
    string Category,
    IFormFile File,
    MediaAccessLevel? AccessLevel,
    string OwnerType,
    string OwnerId);