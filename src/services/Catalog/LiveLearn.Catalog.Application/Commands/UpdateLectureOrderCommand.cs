using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record UpdateLectureOrderCommand(Guid CourseId, Guid TutorId, Guid SectionId, Guid LectureId, int Order) : ICommand;
