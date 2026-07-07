using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Enums;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record AddLectureCommand
(
    Guid CourseId,
    Guid SectionId,
    Guid LectureId,
    Guid TutorId,
    string Title,
    LectureType LectureType,
    int Order,
    TimeSpan Duration
) : ICommand<Guid>;
