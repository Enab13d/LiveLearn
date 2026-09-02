using LiveLearn.BuildingBlocks;
namespace LiveLearn.Assessment.Application.Commands;


public sealed record SubmitHomeworkCommand(Guid StudentId, Guid HomeworkId, string Content) : ICommand;
