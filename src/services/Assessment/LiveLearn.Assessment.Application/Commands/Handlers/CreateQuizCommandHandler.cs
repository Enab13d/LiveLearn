using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class CreateQuizCommandHandler(
    IAssessmentTaskRepository repository,
    IUnitOfWork unitOfWork
) : ICommandHandler<CreateQuizCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateQuizCommand request, CancellationToken ct)
    {
        var quizId = Guid.NewGuid();
        var quiz = Quiz.Create(quizId, request.Title, request.TutorId, request.PassingScore);
        await repository.AddAsync(quiz, ct);
        await unitOfWork.CommitAsync(ct);
        return Result<Guid>.Success(quizId);
    }
}
