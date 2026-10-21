using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;


internal sealed class IssueWatchTicketCommandHandler(
    ICurrentUser currentUser,
    ICourseAccess courseAccess,
    ICourseRepository repository,
    IWatchTicketSigner watchTicketSigner) : ICommandHandler<IssueWatchTicketCommand, WatchTicketDto>
{
    public async Task<Result<WatchTicketDto>> Handle(IssueWatchTicketCommand request, CancellationToken ct)
    {
        var access = await courseAccess.GetLearningAccessAsync(request.CourseId, ct);
        if (access is null) return Result<WatchTicketDto>.Failure(CourseErrors.NotFound);

        var course = await repository.GetByIdAsync(request.CourseId, ct);
        if (course is null) return Result<WatchTicketDto>.Failure(CourseErrors.NotFound);

        var videoId = course.GetPlayableVideoId(request.SectionId, request.LectureId, access);
        if (!videoId.IsSuccess) return Result<WatchTicketDto>.Failure(videoId.Errors);

        return watchTicketSigner.IssueWatchTicket(currentUser.UserId, request.LectureId, videoId.Value);
    }
}
