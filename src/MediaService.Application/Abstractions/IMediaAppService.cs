using MediaService.Application.Commands;
using MediaService.Application.DTOs;

namespace MediaService.Application.Abstractions;

public interface IMediaAppService
{
    Task<Result<MediaResponse>> UploadPersonalAsync(
        UploadPersonalMediaCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<MediaResponse>> UploadAttachmentAsync(
        UploadAttachmentCommand command,
        CancellationToken cancellationToken = default);

    Task<Result> LinkAsync(
        LinkMediaCommand command,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveLinkAsync(
        RemoveMediaLinkCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<MediaDownloadResult>> DownloadAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(
        Guid tenantId,
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Result<MediaValidationResponse>> ValidateAsync(
        ValidateMediaCommand command,
        CancellationToken cancellationToken = default);
}