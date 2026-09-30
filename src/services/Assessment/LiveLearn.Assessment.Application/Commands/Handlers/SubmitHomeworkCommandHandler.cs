using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.Assessment.Domain.Enums;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class SubmitHomeworkCommandHandler(
    IAssessmentTaskRepository assessmentTaskRepository,
    IHomeworkSubmissionRepository homeworkSubmissionRepository,
    IEnrollmentLookup enrollmentLookup,
    IUnitOfWork unitOfWork
) : ICommandHandler<SubmitHomeworkCommand>
{
    public async Task<Result> Handle(SubmitHomeworkCommand request, CancellationToken ct)
    {

        var homework = await assessmentTaskRepository.GetHomeworkAsync(request.HomeworkId, ct);
        if (homework is null) return Result.Failure(TaskErrors.NotFound);

        if (homework.CourseId is not Guid courseId
            || !await enrollmentLookup.IsEnrolledAsync(request.StudentId, courseId, ct))
            return Result.Failure(TaskErrors.Forbidden);

        if (homework.SectionId is not Guid sectionId)
            return Result.Failure(TaskErrors.UnassignedTaskEvaluation);

        var existingSubmissions = await homeworkSubmissionRepository
            .FindAsync(
                e => e.HomeworkId == request.HomeworkId
                && e.Status == HomeworkStatus.PendingReview
                && e.StudentId == request.StudentId,
                ct);

        if (existingSubmissions.Any()) return Result.Failure(HomeworkErrors.SubmissionAlreadyPending);

        var submission = HomeworkSubmission.Create(
            Guid.NewGuid(), request.HomeworkId, request.StudentId, sectionId, courseId, request.Content);

        await homeworkSubmissionRepository.AddAsync(submission, ct);
        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
