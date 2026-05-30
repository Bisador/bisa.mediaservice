using MediaService.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace MediaService.Infrastructure.Storage.LocalStorage;

public sealed class LocalFileStorage : IFileStorage
{
    private const string BucketName = "local";

    private readonly LocalStorageProviderOptions _options;

    public LocalFileStorage(IOptions<LocalStorageProviderOptions> options)
    {
        _options = options.Value;
        Directory.CreateDirectory(_options.BasePath);
    }

    public string ProviderName => nameof(LocalFileStorage);

    public async Task<StoredFileResult> SaveAsync(
        Stream content,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var objectKey = ObjectKeyGenerator.Generate(originalFileName);
        var fullPath = Path.Combine(_options.BasePath, BucketName, objectKey);
        var directory = Path.GetDirectoryName(fullPath)!;
        Directory.CreateDirectory(directory);

        await using var fs = new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);
        await content.CopyToAsync(fs, cancellationToken);

        return new StoredFileResult(BucketName, objectKey, string.Empty);
    }

    public Task<Stream> OpenReadAsync(string bucketName, string objectKey,
        CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_options.BasePath, bucketName, objectKey);

        Stream stream = File.OpenRead(fullPath);

        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string bucketName, string objectKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_options.BasePath, bucketName, objectKey);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}