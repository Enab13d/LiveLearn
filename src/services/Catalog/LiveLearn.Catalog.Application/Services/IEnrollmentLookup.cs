using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Services;


public interface IEnrollmentLookup
{
    public Task<ActiveEnrollmentDto?> GetActiveAsync(Guid studentId, Guid courseId, CancellationToken ct = default);
}
