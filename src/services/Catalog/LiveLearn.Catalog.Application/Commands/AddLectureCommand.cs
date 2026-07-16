using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Enums;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record AddLectureCommand
(
    Guid CourseId,
    Guid SectionId,
    Guid TutorId,
    string Title,
    LectureType LectureType,
    string Description
) : ICommand<Guid>;
