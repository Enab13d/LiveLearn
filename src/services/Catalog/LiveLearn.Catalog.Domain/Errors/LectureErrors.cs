using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Errors;

public static class LectureErrors
{
    public static readonly Error NotFound = Error.NotFound("Lecture.NotFound", "Lecture was not found");

    public static readonly Error InvalidNeighbors = Error.Validation("Lecture.InvalidNeighbors", "PreviousLectureId and NextLectureId must be immediately adjacent siblings in the section");
}
