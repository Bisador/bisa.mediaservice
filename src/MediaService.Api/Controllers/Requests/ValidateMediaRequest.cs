namespace MediaService.Api.Controllers.Requests;

public sealed class ValidateMediaRequest
{
    public IReadOnlyCollection<Guid> MediaIds { get; set; }
        = [];
}