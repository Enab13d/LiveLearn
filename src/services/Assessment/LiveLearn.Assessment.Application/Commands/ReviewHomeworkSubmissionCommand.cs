using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Application.Commands;

public sealed record ReviewHomeworkSubmissionCommand(
    Guid SubmissionId, 
    string Feedback, 
    bool IsAccepted
) : ICommand;
