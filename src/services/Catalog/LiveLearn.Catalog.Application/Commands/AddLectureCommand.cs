using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Enums;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record AddLectureCommand
(
    Guid CourseId,
    Guid SectionId,
    string Title,
    LectureType LectureType,
    string Description
) : ICommand<Guid>;
