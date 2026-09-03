using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record UpdateHomeworkDescriptionCommand(
    Guid HomeworkId, 
    Guid TutorId, 
    string Description
) : ICommand;
