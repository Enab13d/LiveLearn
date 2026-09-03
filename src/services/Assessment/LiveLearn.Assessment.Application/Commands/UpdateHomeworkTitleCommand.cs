using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record UpdateHomeworkTitleCommand(
    Guid HomeworkId, 
    Guid TutorId, 
    string Title
) : ICommand;
