namespace MediaService.Application.Abstractions;

public record FileIsEmpty() : Error(nameof(FileIsEmpty), "File is empty.");

public record FileSizeExceeded() : Error(nameof(FileSizeExceeded), "File size exceeds the allowed limit.");

public record ContentTypeIsNotAllowed : Error
{
    public ContentTypeIsNotAllowed(string contentType) 
        : base(nameof(ContentTypeIsNotAllowed), $"Content type '{contentType}' is not allowed.")
    { 
    } 
}

public record MediaNotFound() : Error(nameof(MediaNotFound), "Media not found.");
public record InvalidFileName() : Error(nameof(InvalidFileName), "Invalid File Name.");

