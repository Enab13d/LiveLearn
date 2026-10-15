using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Errors;

public static class CourseErrors
{
    public static readonly Error NotFound = Error.NotFound("Course.NotFound", "Course was not found");

    public static readonly Error PublishFailed = Error.Conflict("Course.PublishFailed", "Course must be in Draft status to be published");

    public static readonly Error ArchiveFailed = Error.Conflict("Course.ArchiveFailed", "Course must be in Published status to be archived");

    public static readonly Error SectionNotFound = Error.NotFound("Course.SectionNotFound", "Section was not found");

    public static readonly Error Forbidden = Error.Forbidden("Course.Forbidden", "Access to course modification is restricted for this user");

    public static readonly Error InvalidSectionNeighbors = Error.Validation("Course.InvalidSectionNeighbors", "PreviousSectionId and NextSectionId must be immediately adjacent siblings in the course");

    public static readonly Error NoActiveContent = Error.Conflict("Course.NoActiveContent", "Course must have at least one active lecture or task to be published");

    public static readonly Error LastActiveItem = Error.Conflict("Course.LastActiveItem", "A published course must keep at least one active lecture or task");
}
