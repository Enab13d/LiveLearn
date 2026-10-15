using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Errors;

public static class SectionErrors
{
    public static readonly Error TaskNotFound = Error.NotFound("Section.TaskNotFound", "No active assignment of this task was found in the section");

    public static readonly Error TaskAlreadyAssigned = Error.Conflict("Section.TaskAlreadyAssigned", "This task is already assigned to this course");

    public static readonly Error Archived = Error.Conflict("Section.Archived", "Section is archived");
}
