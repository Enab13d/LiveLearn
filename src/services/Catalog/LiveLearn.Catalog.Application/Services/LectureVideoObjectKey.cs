namespace LiveLearn.Catalog.Application.Services;


// Shared by the upload-URL command and the uploaded event so both always point at the same object.
public static class LectureVideoObjectKey
{
    public static string For(Guid courseId, Guid lectureId, Guid videoId) => $"{courseId}/{lectureId}/{videoId}";
}
