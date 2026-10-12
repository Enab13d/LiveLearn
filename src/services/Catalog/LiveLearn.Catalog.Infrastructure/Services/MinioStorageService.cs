using Amazon.S3;
using Amazon.S3.Model;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace LiveLearn.Catalog.Infrastructure.Services;


internal sealed class MinioStorageService(IAmazonS3 s3Client, TimeProvider timeProvider, IOptions<StorageOptions> options) : IStorageService
{
    public async Task<string> GetRawVideoUploadUrlAsync(string objectKey, CancellationToken ct = default)
    {
        var request = new GetPreSignedUrlRequest()
        {
            BucketName = options.Value.RawVideosBucket,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = timeProvider.GetUtcNow().AddMinutes(options.Value.UploadUrlTtlMinutes).UtcDateTime,
            // The SDK defaults to HTTPS regardless of ServiceURL; local MinIO is plain HTTP.
            Protocol = new Uri(options.Value.ServiceUrl).Scheme == Uri.UriSchemeHttps ? Protocol.HTTPS : Protocol.HTTP,
        };
        var url = await s3Client.GetPreSignedURLAsync(request);
        return url;
    }
}
