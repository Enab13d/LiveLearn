using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Errors;

public static class TaskErrors
{
    public static readonly Error Archived = Error.Conflict("Task.Archived", "Archived tasks cannot be assigned to a section");
}
