using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.Errors;


public static class HomeworkErrors
{
    public static Error InvalidStatus =>
        Error.Conflict("HomeworkErrors.InvalidStatus", "Changing status only allowed on HomeworkSubmisson with status 'PendingReview'");

    public static Error SubmissionAlreadyPending =>
        Error.Conflict("HomeworkErrors.SubmissionAlreadyPending", "Unable to submit homework, while it's in Status 'PendingReview'");
}
