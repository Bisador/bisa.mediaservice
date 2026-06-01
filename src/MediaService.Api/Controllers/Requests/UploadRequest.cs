using MediaService.Domain.Enums; 

namespace MediaService.Api.Controllers.Requests;

public record UploadRequest
{
    public required IFormFile File { get; init; }
    public MediaAccessLevel? AccessLevel { get; init; } 
}
 