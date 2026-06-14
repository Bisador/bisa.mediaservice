using MediaService.Application.Abstractions;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace MediaService.Infrastructure.Storage.MinioStorage;

public sealed class MinioFileStorage : IFileStorage
{
    private readonly IMinioClient _minio;
    private readonly MinioStorageProviderOptions _options;

    public MinioFileStorage(IOptions<MinioStorageProviderOptions> configuration)
    {
        _options = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));

        _minio = new MinioClient()
            .WithEndpoint(_options.Endpoint)
            .WithCredentials(_options.AccessKey, _options.SecretKey)
            .WithSSL(_options.UseSSL)
            .Build();
    }

    public string ProviderName => nameof(MinioFileStorage);
    
    public async Task<StoredFileResult> SaveAsync(
        string objectKey, 
        Stream content,
        string originalFileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var ext = Path.GetExtension(originalFileName);
 
        var size = content.CanSeek ? content.Length : -1;

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(size)
            .WithContentType(contentType);

        var response = await _minio.PutObjectAsync(putObjectArgs, cancellationToken);

        var publicUrl =
            $"{_options.Bucket}/{objectKey}";

        return new StoredFileResult(
            _options.Bucket,
            objectKey,
            publicUrl,
            response.Etag);
    }

    public async Task<Stream> OpenReadAsync(
        string bucketName, 
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        var memory = new MemoryStream();

        var args = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectKey) 
            .WithCallbackStream(stream => { stream.CopyTo(memory); });

        await _minio.GetObjectAsync(args, cancellationToken);

        memory.Position = 0;

        return memory;
    }

    public async Task DeleteAsync(
        string bucketName,
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectKey);

        await _minio.RemoveObjectAsync(
            args,
            cancellationToken);
    }
    
    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        var existsArgs = new BucketExistsArgs()
            .WithBucket(_options.Bucket);

        var exists =
            await _minio.BucketExistsAsync(
                existsArgs,
                cancellationToken);

        if (exists)
            return;

        var makeArgs = new MakeBucketArgs()
            .WithBucket(_options.Bucket);

        await _minio.MakeBucketAsync(
            makeArgs,
            cancellationToken);
    }
}