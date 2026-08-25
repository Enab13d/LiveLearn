using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Entities;

namespace LiveLearn.Catalog.Application.Repositories;


public interface ICourseRepository : IRepository<Course, Guid>
{
    Task<Course?> GetByTaskIdAsync(Guid taskId, CancellationToken ct = default);
}
