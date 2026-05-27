using MediaService.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace MediaService.Infrastructure.Storage.MinioStorage;

public sealed class MinioFileStorage : IFileStorage
{
    private readonly IMinioClient _minio;
    private readonly MinioOptions _options; 

    public MinioFileStorage(IOptions<MinioOptions> configuration)
    {
        _options = configuration.Value??throw new ArgumentNullException(nameof(configuration));  
        
        _minio = new MinioClient()
            .WithEndpoint(_options.Endpoint)
            .WithCredentials(_options.AccessKey, _options.SecretKey)
            .WithSSL(_options.UseSSL)
            .Build();
    }

    public async Task<StoredFileResult> SaveAsync(
        Stream content,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(originalFileName);

        var objectName = $"{Guid.NewGuid():N}{ext}";

        var size = content.CanSeek ? content.Length : -1;
        
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectName)
            .WithStreamData(content)
            .WithObjectSize(size)
            .WithContentType(contentType);

        await _minio.PutObjectAsync(putObjectArgs, cancellationToken);

        var publicUrl =
            $"http://{_options.Endpoint}/{_options.Bucket}/{objectName}";

        return new StoredFileResult(
            objectName,
            objectName,
            publicUrl);
    }

    public async Task<Stream> OpenReadAsync(
        string bucketName, string objectKey,
        CancellationToken cancellationToken = default)
    {
        var memory = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(storagePath)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(memory);
            });

        await _minio.GetObjectAsync(args, cancellationToken);

        memory.Position = 0;

        return memory;
    }

    public async Task DeleteAsync(
        string bucketName, string objectKey,
        CancellationToken cancellationToken = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(storagePath);

        await _minio.RemoveObjectAsync(args, cancellationToken);
    }
}