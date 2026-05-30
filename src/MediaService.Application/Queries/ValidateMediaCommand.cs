namespace MediaService.Application.Queries;

public sealed record ValidateMediaCommand(IReadOnlyCollection<Guid> MediaIds);