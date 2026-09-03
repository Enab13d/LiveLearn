using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class CreateHomeworkCommandHandler(IAssessmentTaskRepository repository, IUnitOfWork unitOfWork) : ICommandHandler<CreateHomeworkCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateHomeworkCommand request, CancellationToken ct)
    {
        var (tutorId, title, description) = request;
        var homeworkId = Guid.NewGuid();
        var homework = Homework.Create(homeworkId, title, tutorId, description);
        await repository.AddAsync(homework, ct);
        await unitOfWork.CommitAsync(ct);
        return homeworkId;
    }
}
