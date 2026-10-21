using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class CreateHomeworkCommandHandler(ICurrentUser currentUser, IAssessmentTaskRepository repository, IUnitOfWork unitOfWork) : ICommandHandler<CreateHomeworkCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateHomeworkCommand request, CancellationToken ct)
    {
        var (title, description) = request;
        var homeworkId = Guid.NewGuid();
        var homework = Homework.Create(homeworkId, title, currentUser.UserId, description);
        await repository.AddAsync(homework, ct);
        await unitOfWork.CommitAsync(ct);
        return homeworkId;
    }
}
