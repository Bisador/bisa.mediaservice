using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace MediaService.Infrastructure.Storage.MinioStorage;

public sealed class MinioInitializer
{
    private readonly IMinioClient _minio;

    private readonly MinioStorageProviderOptions _options;

    public MinioInitializer(IOptions<MinioStorageProviderOptions> configuration)
    {
        _options = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));

        _minio = new MinioClient()
            .WithEndpoint(_options.Endpoint)
            .WithCredentials(
                _options.AccessKey,
                _options.SecretKey)
            .WithSSL(_options.UseSSL)
            .Build();
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        var existsArgs = new BucketExistsArgs().WithBucket(_options.Bucket);

        var exists = await _minio.BucketExistsAsync(existsArgs, cancellationToken);

        if (exists)
            return;

        var makeArgs = new MakeBucketArgs().WithBucket(_options.Bucket);

        await _minio.MakeBucketAsync(makeArgs, cancellationToken);
    }
}