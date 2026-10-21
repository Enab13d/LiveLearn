using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Enums;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Domain.Entities;

public sealed class Lecture : Entity<Guid>
{
    private Lecture() { }
    internal Lecture(Guid id, Guid sectionId, string title, LectureType type, int order, string description, long createdRevision)
    {
        Id = id;
        CreatedRevision = createdRevision;
        SectionId = sectionId;
        Title = title;
        Type = type;
        Order = order;
        Description = description;
    }

    public Guid SectionId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public LectureType Type { get; private set; }

    public int Order { get; private set; }

    public TimeSpan? Duration { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public Guid? VideoId { get; private set; }

    public VideoStatus VideoStatus { get; private set; } = VideoStatus.None;

    public long CreatedRevision { get; private set; }

    public long? ArchivedRevision { get; private set; } = null;

    internal void Update(string title, string description)
    {
        Title = title;
        Description = description;
    }

    internal void SetOrder(int order)
    {
        Order = order;
    }

    internal void MoveTo(Guid sectionId, int order)
    {
        SectionId = sectionId;
        Order = order;
    }

    internal Result RequestVideoUpload(Guid videoId)
    {
        if (Type != LectureType.Video) return Result.Failure(LectureErrors.NotVideo);
        if (VideoStatus is VideoStatus.Processing or VideoStatus.Ready)
            return Result.Failure(LectureErrors.VideoUploadNotAllowed);

        VideoId = videoId;
        VideoStatus = VideoStatus.Uploading;
        return Result.Success();
    }

    internal Result MarkVideoUploaded(Guid videoId)
    {
        if (VideoStatus != VideoStatus.Uploading || VideoId != videoId)
            return Result.Failure(LectureErrors.VideoNotUploading);

        VideoStatus = VideoStatus.Processing;
        return Result.Success();
    }

    internal void MarkVideoReady(Guid videoId, TimeSpan duration)
    {
        if (VideoId != videoId || VideoStatus != VideoStatus.Processing) return;
        VideoStatus = VideoStatus.Ready;
        Duration = duration;
    }

    internal void MarkVideoFailed(Guid videoId)
    {
        if (VideoId != videoId || VideoStatus != VideoStatus.Processing) return;
        VideoStatus = VideoStatus.Failed;
    }

    internal bool IsActive() => ArchivedRevision is null;

    internal bool IsVisibleAt(long revision) =>
        CreatedRevision <= revision && (ArchivedRevision is null || ArchivedRevision > revision);

    internal void Archive(long revision)
    {
        ArchivedRevision ??= revision;
    }
}
