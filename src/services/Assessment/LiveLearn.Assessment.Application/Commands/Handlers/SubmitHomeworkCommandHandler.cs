using LiveLearn.Assessment.Application.Repositories;
using LiveLearn.Assessment.Application.Services;
using LiveLearn.Assessment.Domain.Entities;
using LiveLearn.Assessment.Domain.Enums;
using LiveLearn.Assessment.Domain.Errors;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands.Handlers;


internal sealed class SubmitHomeworkCommandHandler(
    ICurrentUser currentUser,
    IAssessmentTaskRepository assessmentTaskRepository,
    IHomeworkSubmissionRepository homeworkSubmissionRepository,
    IEnrollmentLookup enrollmentLookup,
    ICourseTaskLookup courseTaskLookup,
    IUnitOfWork unitOfWork
) : ICommandHandler<SubmitHomeworkCommand>
{
    public async Task<Result> Handle(SubmitHomeworkCommand request, CancellationToken ct)
    {
        var studentId = currentUser.UserId;

        var homework = await assessmentTaskRepository.GetHomeworkAsync(request.HomeworkId, ct);
        if (homework is null) return Result.Failure(TaskErrors.NotFound);

        // archived tasks stay submittable
        if (!await enrollmentLookup.IsEnrolledAsync(studentId, request.CourseId, ct)
            || !await courseTaskLookup.WasEverAssignedAsync(request.CourseId, homework.Id, ct))
            return Result.Failure(TaskErrors.Forbidden);

        var existingSubmissions = await homeworkSubmissionRepository
            .FindAsync(
                e => e.HomeworkId == request.HomeworkId
                && e.Status == HomeworkStatus.PendingReview
                && e.StudentId == studentId,
                ct);

        if (existingSubmissions.Any()) return Result.Failure(HomeworkErrors.SubmissionAlreadyPending);

        var submission = HomeworkSubmission.Create(
            Guid.NewGuid(), request.HomeworkId, studentId, request.CourseId, request.Content);

        await homeworkSubmissionRepository.AddAsync(submission, ct);
        await unitOfWork.CommitAsync(ct);

        return Result.Success();
    }
}
