namespace MediaService.Application.Commands;

public sealed record RemoveMediaLinkCommand(
    Guid TenantId,
    Guid MediaId,
    Guid LinkId);