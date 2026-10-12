namespace LiveLearn.Catalog.Application.Services;


public interface IStorageService
{
    // Presigned PUT URL for the raw-videos bucket; the client uploads directly to MinIO.
    public Task<string> GetRawVideoUploadUrlAsync(string objectKey, CancellationToken ct = default);
}
