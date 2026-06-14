using MediaService.Domain.Enums;

namespace MediaService.Api.Controllers.Requests;

public sealed record UploadAttachmentBatchRequest(
    string Category,
    List<IFormFile> File,
    MediaAccessLevel? AccessLevel);