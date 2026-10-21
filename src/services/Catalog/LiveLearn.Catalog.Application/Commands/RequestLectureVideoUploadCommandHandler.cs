using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;
using LiveLearn.Catalog.Application.Repositories;
using LiveLearn.Catalog.Application.Services;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Application.Commands;

internal sealed class RequestLectureVideoUploadCommandHandler(
    ICurrentUser currentUser,
    ICourseRepository courseRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork) : ICommandHandler<RequestLectureVideoUploadCommand, VideoUploadUrlDto>
{
    public async Task<Result<VideoUploadUrlDto>> Handle(RequestLectureVideoUploadCommand request, CancellationToken ct)
    {
        var course = await courseRepository.GetByIdAsync(request.CourseId, ct);

        if (course is null) return Result<VideoUploadUrlDto>.Failure(CourseErrors.NotFound);

        if (course.TutorId != currentUser.UserId) return Result<VideoUploadUrlDto>.Failure(CourseErrors.Forbidden);

        var videoId = Guid.NewGuid();

        var result = course.RequestLectureVideoUpload(request.LectureId, videoId);

        if (!result.IsSuccess) return Result<VideoUploadUrlDto>.Failure(result.FirstError);

        await unitOfWork.CommitAsync(ct);

        var objectKey = LectureVideoObjectKey.For(request.CourseId, request.LectureId, videoId);
        var uploadUrl = await storageService.GetRawVideoUploadUrlAsync(objectKey, ct);

        return Result<VideoUploadUrlDto>.Success(new VideoUploadUrlDto(videoId, uploadUrl));
    }
}
