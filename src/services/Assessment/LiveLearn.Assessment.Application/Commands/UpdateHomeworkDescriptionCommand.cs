using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record UpdateHomeworkDescriptionCommand(
    Guid HomeworkId, 
    string Description
) : ICommand;
