namespace MediaService.Application.Abstractions;

public record FileIsEmpty() : Error(nameof(FileIsEmpty), "File is empty.");

public record FileSizeExceededError() : Error(nameof(FileSizeExceededError), "File size exceeds the allowed limit.");

public record ContentTypeIsNotAllowed : Error
{
    public ContentTypeIsNotAllowed(string contentType) 
        : base(nameof(ContentTypeIsNotAllowed), $"Content type '{contentType}' is not allowed.")
    { 
    } 
}

public record MediaNotFoundError() : Error(nameof(MediaNotFoundError), "Media not found.");
public record AccessDeniedError() : Error(nameof(AccessDeniedError), "Access Denied.");
public record InvalidFileName() : Error(nameof(InvalidFileName), "Invalid File Name.");

public record DuplicateLinkError() : Error(nameof(DuplicateLinkError), "Link is Duplicated..");
