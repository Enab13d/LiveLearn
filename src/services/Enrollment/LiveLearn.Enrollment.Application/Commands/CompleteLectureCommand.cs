using LiveLearn.BuildingBlocks;

namespace LiveLearn.Enrollment.Application.Commands;

// Sent by the complete-lecture endpoint; the student is the caller (ICurrentUser).
public sealed record CompleteLectureCommand(Guid CourseId, Guid LectureId) : ICommand;
