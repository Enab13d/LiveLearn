using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record RequestLectureVideoUploadCommand(Guid CourseId, Guid LectureId) : ICommand<VideoUploadUrlDto>;
