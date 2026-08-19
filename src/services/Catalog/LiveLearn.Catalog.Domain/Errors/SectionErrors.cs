using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Errors;

public static class SectionErrors
{
    public static readonly Error TaskNotFound = Error.NotFound("Section.TaskNotFound", "Section task was not found");

    public static readonly Error TaskAlreadyAssigned = Error.Conflict("Section.TaskAlreadyAssigned", "This task is already assigned to this section");
}
