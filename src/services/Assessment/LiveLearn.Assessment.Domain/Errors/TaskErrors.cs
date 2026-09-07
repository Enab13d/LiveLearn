using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.Errors;


public static class TaskErrors
{
    public static Error Forbidden => Error.Forbidden("TaskErrors.Forbidden", "Access to this task is forbidden");

    public static Error NotFound => Error.NotFound("TaskErrors.NotFound", "Task not found");

    public static Error Locked => Error.Conflict("TaskErrors.Locked", "Task is locked");

    public static Error UnassignedTaskEvaluation => Error.Conflict("TaskErrors.UnassignedTaskEvaluation", "Task must be assigned to course and section to be evaluated");
}
