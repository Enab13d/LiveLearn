using LiveLearn.BuildingBlocks;
using LiveLearn.Enrollment.Domain.Entities;

namespace LiveLearn.Enrollment.Application.Repositories;


public interface IEnrollmentRepository : IRepository<CourseEnrollment, Guid>
{
    // Loads the enrollment with its items; at most one exists per (StudentId, CourseId).
    Task<CourseEnrollment?> GetByStudentAndCourseAsync(Guid studentId, Guid courseId, CancellationToken ct = default);

    // Untracked and without items: for read-only use such as republishing snapshots.
    Task<IReadOnlyList<CourseEnrollment>> GetAllAsync(CancellationToken ct = default);
}
