using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record UpdateHomeworkTitleCommand(
    Guid HomeworkId, 
    string Title
) : ICommand;
