using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Common;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Catalog.Domain.Enums;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Domain.Entities;


public sealed class Course : AggregateRoot<Guid>
{
    private const int GapSize = 1000;
    private const int MinGapBeforeRebalance = 1;

    private Course() { }

    public Guid CategoryId { get; private set; }

    public Guid TutorId { get; private set; }

    public long Revision { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public string ThumbnailUrl { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public CourseStatus Status { get; private set; }

    private readonly List<Section> _sections = [];

    public IReadOnlyCollection<Section> Sections => _sections.AsReadOnly();

    public static Course Create(Guid id, Guid tutorId, Guid categoryId, string title, string description, decimal price)
    {
        var course = new Course()
        {
            Id = id,
            TutorId = tutorId,
            CategoryId = categoryId,
            Title = title,
            Description = description,
            Price = price,
            Status = CourseStatus.Draft
        };
        course.RaiseDomainEvent(new CourseCreatedDomainEvent(id, tutorId, categoryId, title));

        return course;
    }

    public void Update(string title, string description, decimal price, Guid categoryId)
    {
        if (Status == CourseStatus.Published && price != Price)
            RaiseDomainEvent(new CoursePriceChangedDomainEvent(Id, price));

        Title = title;
        Description = description;
        Price = price;
        CategoryId = categoryId;

    }

    public Result Publish(DateTimeOffset publishedAt)
    {
        if (Status != CourseStatus.Draft) return Result.Failure(CourseErrors.PublishFailed);

        if (ActiveItemsCount() == 0) return Result.Failure(CourseErrors.NoActiveContent);

        Status = CourseStatus.Published;
        var taskIds = _sections
            .SelectMany(s => s.SectionTasks.Where(t => t.IsActive()).Select(t => t.TaskId))
            .ToList();

        RaiseDomainEvent(new CoursePublishedDomainEvent(Id, TutorId, Price, taskIds, publishedAt));
        return Result.Success();
    }

    public Result Archive()
    {
        if (Status != CourseStatus.Published) return Result.Failure(CourseErrors.ArchiveFailed);
        Status = CourseStatus.Archived;
        RaiseDomainEvent(new CourseArchivedDomainEvent(Id));
        return Result.Success();
    }

    public Result AddLecture(Guid sectionId, Guid lectureId, string title, LectureType lectureType, string description)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);
        if (!section.IsActive()) return Result.Failure(SectionErrors.Archived);

        var revision = NextRevision();
        var lecture = section.AddLecture(lectureId, title, lectureType, description, revision);
        RaiseDomainEvent(new LectureAddedDomainEvent(Id, sectionId, lecture.Id, revision));
        return Result.Success();
    }

    public Result AddSection(Guid sectionId, string title)
    {
        int lastOrder = _sections.Count == 0 ? 0 : _sections.Max(s => s.Order);
        int order = lastOrder + GapSize;
        var revision = NextRevision();
        Section section = new(sectionId, Id, title, order, revision);
        _sections.Add(section);
        RaiseDomainEvent(new SectionAddedDomainEvent(Id, sectionId, revision));
        return Result.Success();
    }

    public Result<OrderUpdateResult> UpdateLectureOrder(Guid sectionId, Guid lectureId, Guid? previousLectureId, Guid? nextLectureId)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);

        if (section is null) return Result<OrderUpdateResult>.Failure(CourseErrors.SectionNotFound);

        return section.UpdateLectureOrder(lectureId, previousLectureId, nextLectureId);
    }

    public Result UpdateSection(Guid sectionId, string title)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);

        section.Update(title);
        RaiseDomainEvent(new SectionUpdatedDomainEvent(Id, sectionId));
        return Result.Success();
    }

    public Result<OrderUpdateResult> UpdateSectionOrder(Guid sectionId, Guid? previousSectionId, Guid? nextSectionId)
    {
        var currentSection = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (currentSection is null) return Result<OrderUpdateResult>.Failure(CourseErrors.SectionNotFound);

        if (currentSection.Id == previousSectionId || currentSection.Id == nextSectionId || previousSectionId == nextSectionId)
            return Result<OrderUpdateResult>.Failure(CourseErrors.InvalidSectionNeighbors);

        // Move to the very start: no previous sibling
        if (previousSectionId is null && nextSectionId is not null)
        {
            var nextSection = _sections.FirstOrDefault(s => s.Id == nextSectionId);
            if (nextSection is null) return Result<OrderUpdateResult>.Failure(CourseErrors.SectionNotFound);

            // nextSection must be first, otherwise some other section already
            // sits before it and the new order below would collide with that section's order.
            bool nextIsNotFirst = _sections.Any(s => s.Id != currentSection.Id && s.Order < nextSection.Order);
            if (nextIsNotFirst) return Result<OrderUpdateResult>.Failure(CourseErrors.InvalidSectionNeighbors);

            // Not enough room between position 0 and nextSection to fit a value in between - rebalance first.
            if (nextSection.Order <= MinGapBeforeRebalance)
            {
                RebalanceSections(GapSize);
                nextSection = _sections.First(s => s.Id == nextSectionId);
                currentSection.SetOrder(nextSection.Order / 2);
                return BuildOrderUpdateResult(currentSection, rebalanced: true);
            }

            currentSection.SetOrder(nextSection.Order / 2);
            return BuildOrderUpdateResult(currentSection, rebalanced: false);
        }

        // Move to the very end: no next sibling
        if (previousSectionId is not null && nextSectionId is null)
        {
            var previousSection = _sections.FirstOrDefault(s => s.Id == previousSectionId);
            if (previousSection is null) return Result<OrderUpdateResult>.Failure(CourseErrors.SectionNotFound);

            // previousSection must be last, otherwise some other section already
            // sits after it and the new order below would collide with that section's order.
            bool previousIsNotLast = _sections.Any(s => s.Id != currentSection.Id && s.Order > previousSection.Order);
            if (previousIsNotLast) return Result<OrderUpdateResult>.Failure(CourseErrors.InvalidSectionNeighbors);

            // Appending past the last section always has a full gap of room above it - never rebalances.
            currentSection.SetOrder(previousSection.Order + GapSize);
            return BuildOrderUpdateResult(currentSection, rebalanced: false);
        }

        // Move between two sections: both neighbors are given.
        if (previousSectionId is not null && nextSectionId is not null)
        {
            var previousSection = _sections.FirstOrDefault(s => s.Id == previousSectionId);
            var nextSection = _sections.FirstOrDefault(s => s.Id == nextSectionId);
            if (previousSection is null || nextSection is null) return Result<OrderUpdateResult>.Failure(CourseErrors.SectionNotFound);

            if (previousSection.Order >= nextSection.Order)
                return Result<OrderUpdateResult>.Failure(CourseErrors.InvalidSectionNeighbors);

            // previousSection and nextSection must be adjacent siblings, otherwise some
            // other section sits between them and the new order below would collide with it.
            bool areAdjacent = !_sections.Any(s =>
                s.Id != currentSection.Id && s.Order > previousSection.Order && s.Order < nextSection.Order);
            if (!areAdjacent) return Result<OrderUpdateResult>.Failure(CourseErrors.InvalidSectionNeighbors);

            // Not enough room between the two neighbors to fit a value in between - rebalance first.
            if (nextSection.Order - previousSection.Order <= MinGapBeforeRebalance)
            {
                RebalanceSections(GapSize);
                previousSection = _sections.First(s => s.Id == previousSectionId);
                nextSection = _sections.First(s => s.Id == nextSectionId);
                currentSection.SetOrder((previousSection.Order + nextSection.Order) / 2);
                return BuildOrderUpdateResult(currentSection, rebalanced: true);
            }

            currentSection.SetOrder((previousSection.Order + nextSection.Order) / 2);
            return BuildOrderUpdateResult(currentSection, rebalanced: false);
        }

        // Neither neighbor given - nothing to position relative to.
        return Result<OrderUpdateResult>.Failure(CourseErrors.InvalidSectionNeighbors);
    }

    private Result<OrderUpdateResult> BuildOrderUpdateResult(Section movedSection, bool rebalanced)
    {
        var snapshot = rebalanced ? _sections.ToDictionary(s => s.Id, s => s.Order) : null;
        return new OrderUpdateResult(movedSection.Id, movedSection.Order, rebalanced, snapshot);
    }

    private void RebalanceSections(int gapSize)
    {
        var sections = _sections.OrderBy(s => s.Order).ToList();
        for (int i = 0; i < sections.Count; i++)
        {
            sections[i].SetOrder((i + 1) * gapSize);
        }
    }

    public Result ArchiveSection(Guid sectionId)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);
        if (!section.IsActive()) return Result.Failure(SectionErrors.Archived);
        if (WouldLeaveNoActiveItems(section.ActiveItemsCount())) return Result.Failure(CourseErrors.LastActiveItem);

        var revision = NextRevision();
        var archived = section.Archive(revision);
        RaiseDomainEvent(new SectionArchivedDomainEvent(Id, sectionId, revision, archived.LectureIds, archived.TaskIds));
        return Result.Success();
    }

    public Result ArchiveLecture(Guid sectionId, Guid lectureId)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);

        var lecture = section.Lectures.FirstOrDefault(l => l.Id == lectureId);
        if (lecture is null) return Result.Failure(LectureErrors.NotFound);
        if (!lecture.IsActive()) return Result.Failure(LectureErrors.Archived);
        if (WouldLeaveNoActiveItems(1)) return Result.Failure(CourseErrors.LastActiveItem);

        var revision = NextRevision();
        lecture.Archive(revision);
        RaiseDomainEvent(new LectureArchivedDomainEvent(Id, lectureId, revision));
        return Result.Success();
    }

    // Minor change: never bumps Revision. Appends to the end of the target section.
    public Result MoveLecture(Guid sectionId, Guid lectureId, Guid targetSectionId)
    {
        var source = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (source is null) return Result.Failure(CourseErrors.SectionNotFound);

        var lecture = source.Lectures.FirstOrDefault(l => l.Id == lectureId);
        if (lecture is null) return Result.Failure(LectureErrors.NotFound);

        var target = _sections.FirstOrDefault(s => s.Id == targetSectionId);
        if (target is null) return Result.Failure(CourseErrors.SectionNotFound);
        if (!target.IsActive()) return Result.Failure(SectionErrors.Archived);
        if (source.Id == target.Id) return Result.Success();
        if (!lecture.IsActive()) return Result.Failure(LectureErrors.Archived);

        source.DetachLecture(lecture);
        target.AttachLecture(lecture);
        return Result.Success();
    }

    public Result AssignTaskToSection(Guid sectionId, Guid taskId, string taskType)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);
        if (!section.IsActive()) return Result.Failure(SectionErrors.Archived);
        if (_sections.Any(s => s.HasActiveTask(taskId))) return Result.Failure(SectionErrors.TaskAlreadyAssigned);

        var revision = NextRevision();
        section.AddTask(taskId, Id, taskType, revision);
        RaiseDomainEvent(new TaskAssignedToSectionDomainEvent(taskId, section.Id, Id, revision));
        return Result.Success();
    }

    public Result UnassignTaskFromSection(Guid sectionId, Guid taskId)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);
        if (!section.HasActiveTask(taskId)) return Result.Failure(SectionErrors.TaskNotFound);
        if (WouldLeaveNoActiveItems(1)) return Result.Failure(CourseErrors.LastActiveItem);

        var revision = NextRevision();
        section.UnassignTask(taskId, revision);
        RaiseDomainEvent(new TaskUnassignedFromSectionDomainEvent(taskId, section.Id, Id, revision));
        return Result.Success();
    }

    public Result RequestLectureVideoUpload(Guid lectureId, Guid videoId)
    {
        var lecture = FindLecture(lectureId);
        if (lecture is null) return Result.Failure(LectureErrors.NotFound);

        return lecture.RequestVideoUpload(videoId);
    }

    public Result MarkLectureVideoUploaded(Guid lectureId, Guid videoId)
    {
        var lecture = FindLecture(lectureId);
        if (lecture is null) return Result.Failure(LectureErrors.NotFound);

        var result = lecture.MarkVideoUploaded(videoId);
        if (result.IsSuccess) RaiseDomainEvent(new LectureVideoUploadedDomainEvent(Id, lecture.Id, videoId));
        return result;
    }

    public Result MarkLectureVideoReady(Guid lectureId, Guid videoId, TimeSpan duration)
    {
        var lecture = FindLecture(lectureId);
        if (lecture is null) return Result.Failure(LectureErrors.NotFound);

        lecture.MarkVideoReady(videoId, duration);
        return Result.Success();
    }

    public Result MarkLectureVideoFailed(Guid lectureId, Guid videoId)
    {
        var lecture = FindLecture(lectureId);
        if (lecture is null) return Result.Failure(LectureErrors.NotFound);

        lecture.MarkVideoFailed(videoId);
        return Result.Success();
    }

    public Result<Guid> GetPlayableVideoId(Guid sectionId, Guid lectureId)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result<Guid>.Failure(CourseErrors.SectionNotFound);

        var lecture = section.Lectures.FirstOrDefault(l => l.Id == lectureId);
        if (lecture is null) return Result<Guid>.Failure(LectureErrors.NotFound);
        if (lecture.Type != LectureType.Video) return Result<Guid>.Failure(LectureErrors.NotVideo);
        if (lecture.VideoStatus != VideoStatus.Ready || lecture.VideoId is not Guid videoId)
            return Result<Guid>.Failure(LectureErrors.VideoNotReady);

        return Result<Guid>.Success(videoId);
    }

    private Lecture? FindLecture(Guid lectureId) =>
        _sections.SelectMany(s => s.Lectures).FirstOrDefault(l => l.Id == lectureId);

    private long NextRevision()
    {
        Revision++;
        return Revision;
    }

    private int ActiveItemsCount() => _sections.Where(s => s.IsActive()).Sum(s => s.ActiveItemsCount());

    // A published course must keep at least one active lecture or task.
    private bool WouldLeaveNoActiveItems(int archivedCount) =>
        Status == CourseStatus.Published && ActiveItemsCount() - archivedCount <= 0;
}
