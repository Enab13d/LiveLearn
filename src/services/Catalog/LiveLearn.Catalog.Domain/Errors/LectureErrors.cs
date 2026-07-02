using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Errors;

public static class LectureErrors
{
    public static readonly Error NotFound = Error.NotFound("Lecture.NotFound", "Lecture was not found");
}
