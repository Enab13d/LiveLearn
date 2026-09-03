using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.Assessment.Domain.Enums;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class SubmitHomeworkCommandHandler(
    IAssessmentTaskRepository assessmentTaskRepository,
    IHomeworkSubmissionRepository homeworkSubmissionRepository,
    IUnitOfWork unitOfWork
) : ICommandHandler<SubmitHomeworkCommand>
{
    public async Task<Result> Handle(SubmitHomeworkCommand request, CancellationToken ct)
    {

        var homework = await assessmentTaskRepository.GetHomeworkAsync(request.HomeworkId, ct);
        if (homework is null) return Result.Failure(TaskErrors.NotFound);

        var existingSubmissions = await homeworkSubmissionRepository
            .FindAsync(
                e => e.HomeworkId == request.HomeworkId
                && e.Status == HomeworkStatus.PendingReview
                && e.StudentId == request.StudentId, 
                ct);

        if (existingSubmissions.Any()) return Result.Failure(HomeworkErrors.SubmissionAlreadyPending);

        var submission = HomeworkSubmission.Create(
            Guid.NewGuid(), request.HomeworkId, request.StudentId, homework.SectionId, homework.CourseId, request.Content);

        await homeworkSubmissionRepository.AddAsync(submission, ct);
        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
