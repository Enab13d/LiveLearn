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
        Title = title;
        Description = description;
        Price = price;
        CategoryId = categoryId;
    }

    public Result Publish(DateTimeOffset publishedAt)
    {
        if (Status != CourseStatus.Draft) return Result.Failure(CourseErrors.PublishFailed);

        Status = CourseStatus.Published;
        var taskIds = Sections.SelectMany(s => s.SectionTasks.Select(t => t.TaskId)).ToList();
        RaiseDomainEvent(new CoursePublishedDomainEvent(Id, TutorId, taskIds, publishedAt));
        return Result.Success();
    }

    public Result AddLecture(Guid sectionId, Guid lectureId, string title, LectureType lectureType, string description)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);
        var lecture = section.AddLecture(lectureId, title, lectureType, description);
        RaiseDomainEvent(new LectureAddedDomainEvent(Id, sectionId, lecture.Id));
        return Result.Success();
    }

    public Result AddSection(Guid sectionId, string title)
    {
        int lastOrder = _sections.Count == 0 ? 0 : _sections.Max(s => s.Order);
        int order = lastOrder + GapSize;
        Section section = new(sectionId, Id, title, order);
        _sections.Add(section);
        RaiseDomainEvent(new SectionAddedDomainEvent(sectionId, Id));
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

    public Result DeleteSection(Guid sectionId)
    {
        if (Status == CourseStatus.Published) return Result.Failure(CourseErrors.PublishFailed);
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);

        var tasksIds = section.SectionTasks
                .Select(e => e.TaskId)
                .ToList()
                .AsReadOnly();

        _sections.Remove(section);

        RaiseDomainEvent(new SectionRemovedDomainEvent(Id, sectionId, tasksIds));

        return Result.Success();
    }

    public Result AssignTaskToSection(Guid sectionId, Guid taskId, string taskType)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);
        if (section.HasTask(taskId)) return Result.Failure(SectionErrors.TaskAlreadyAssigned);
        var result = section.AddTask(taskId, Id, taskType);
        if (result.IsSuccess) RaiseDomainEvent(new TaskAssignedToSectionDomainEvent(taskId, section.Id, CourseId: Id));
        return result;
    }

    public Result RemoveTaskFromSection(Guid sectionId, Guid taskId)
    {
        if (Status == CourseStatus.Published) return Result.Failure(CourseErrors.TaskRemovalBlocked);
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);
        var result = section.RemoveTask(taskId);
        
        if (result.IsSuccess) 
            RaiseDomainEvent(new TaskRemovedFromSectionDomainEvent(taskId, section.Id, CourseId: Id));

        return result;
    }

}
