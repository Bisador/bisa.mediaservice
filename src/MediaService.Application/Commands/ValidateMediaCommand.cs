namespace MediaService.Application.Commands;

public sealed record ValidateMediaCommand(
    IReadOnlyCollection<Guid> MediaIds);