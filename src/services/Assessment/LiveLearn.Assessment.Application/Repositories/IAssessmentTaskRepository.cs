using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Repositories;


public interface IAssessmentTaskRepository : IRepository<AssessmentTask, Guid>
{
    Task<Homework?> GetHomeworkAsync(Guid id, CancellationToken ct = default);

    Task<Quiz?> GetQuizAsync(Guid id, CancellationToken ct = default);
}
