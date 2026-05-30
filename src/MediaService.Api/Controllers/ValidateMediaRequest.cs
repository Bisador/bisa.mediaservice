namespace MediaService.Api.Controllers;

public sealed class ValidateMediaRequest
{
    public IReadOnlyCollection<Guid> MediaIds { get; set; }
        = [];
}