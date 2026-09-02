using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record UpdateHomeworkCommand(
    Guid HomeworkId, 
    Guid TutorId, 
    string Title, 
    string Description
) : ICommand;
