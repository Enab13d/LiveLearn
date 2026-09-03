using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class ReviewHomeworkSubmissionCommandHandler(
    IHomeworkSubmissionRepository homeworkSubmissionRepository,
    IAssessmentTaskRepository assessmentTaskRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<ReviewHomeworkSubmissionCommand>
{
    public async Task<Result> Handle(ReviewHomeworkSubmissionCommand request, CancellationToken ct)
    {
        var submission = await homeworkSubmissionRepository.GetByIdAsync(request.SubmissionId, ct);
        if (submission is null) return Result.Failure(HomeworkErrors.SubmissionNotFound);
        var homework = await assessmentTaskRepository.GetHomeworkAsync(submission.HomeworkId, ct);
        if (homework is null)
        {
            return Result.Failure(TaskErrors.NotFound);
        }
        if (homework.TutorId != request.TutorId)
            return Result.Failure(TaskErrors.Forbidden);

        var result = submission.Review(request.Feedback, request.IsAccepted);
        if(result.IsSuccess) await unitOfWork.CommitAsync(ct);
        
        return result;

    }
}
