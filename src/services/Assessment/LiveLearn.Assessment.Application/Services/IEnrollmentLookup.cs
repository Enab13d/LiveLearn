namespace LiveLearn.Assessment.Application.Services;


public interface IEnrollmentLookup
{
    public Task<bool> IsEnrolledAsync(Guid userId, Guid courseId);

}
