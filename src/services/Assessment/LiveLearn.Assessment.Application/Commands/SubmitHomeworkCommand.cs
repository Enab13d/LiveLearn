using LiveLearn.BuildingBlocks;
namespace LiveLearn.Assessment.Application.Commands;


public sealed record SubmitHomeworkCommand(Guid StudentId, Guid CourseId, Guid HomeworkId, string Content) : ICommand;
