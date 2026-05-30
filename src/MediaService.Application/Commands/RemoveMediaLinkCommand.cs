namespace MediaService.Application.Commands;

public sealed record RemoveMediaLinkCommand(
    Guid MediaId,
    Guid LinkId);