using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Errors;

public static class LectureErrors
{
    public static readonly Error NotFound = Error.NotFound("Lecture.NotFound", "Lecture was not found");

    public static readonly Error InvalidNeighbors = Error.Validation("Lecture.InvalidNeighbors", "PreviousLectureId and NextLectureId must be immediately adjacent siblings in the section");

    public static readonly Error NotVideo = Error.Validation("Lecture.NotVideo", "Only video lectures accept video uploads");

    public static readonly Error VideoUploadNotAllowed = Error.Conflict("Lecture.VideoUploadNotAllowed", "Lecture video is already processing or ready");

    public static readonly Error VideoNotUploading = Error.Conflict("Lecture.VideoNotUploading", "No pending upload matches this video");
}
