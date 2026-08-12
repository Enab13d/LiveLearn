using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Common;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record UpdateLectureOrderCommand(
    Guid CourseId,
    Guid TutorId,
    Guid SectionId,
    Guid LectureId,
    Guid? PreviousLectureId,
    Guid? NextLectureId) : ICommand<OrderUpdateResult>;
