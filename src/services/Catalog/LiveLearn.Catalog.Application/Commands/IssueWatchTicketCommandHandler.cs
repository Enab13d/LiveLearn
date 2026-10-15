using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;


internal sealed class IssueWatchTicketCommandHandler(
    ICourseRepository repository,
    ICourseContentAccess contentAccess,
    IWatchTicketSigner watchTicketSigner) : ICommandHandler<IssueWatchTicketCommand, WatchTicketDto>
{
    public async Task<Result<WatchTicketDto>> Handle(IssueWatchTicketCommand request, CancellationToken ct)
    {
        var course = await repository.GetByIdAsync(request.CourseId, ct);
        if (course is null) return Result<WatchTicketDto>.Failure(CourseErrors.NotFound);

        if (!await contentAccess.CanAccessAsync(request.UserId, request.IsAdmin, course.TutorId, course.Id, ct))
            return Result<WatchTicketDto>.Failure(LectureErrors.Forbidden);

        var videoId = course.GetPlayableVideoId(request.LectureId);
        if (!videoId.IsSuccess) return Result<WatchTicketDto>.Failure(videoId.Errors);

        return watchTicketSigner.IssueWatchTicket(request.UserId, request.LectureId, videoId.Value);
    }
}
