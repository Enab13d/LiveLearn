using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Common;
using LiveLearn.Catalog.Domain.Enums;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Domain.Entities;


public sealed class Section : Entity<Guid>
{
    private const int GapSize = 1000;
    private const int MinGapBeforeRebalance = 1;

    private Section() { }

    internal Section(Guid id, Guid courseId, string title, int order)
    {
        Id = id;
        CourseId = courseId;
        Title = title;
        Order = order;
    }

    public Guid CourseId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public int Order { get; private set; }
    private readonly List<Lecture> _lectures = [];
    public IReadOnlyCollection<Lecture> Lectures => _lectures.AsReadOnly();
    private readonly List<SectionTask> _sectionTasks = [];
    public IReadOnlyCollection<SectionTask> SectionTasks => _sectionTasks.AsReadOnly();

    internal void Update(string title)
    {
        Title = title;
    }

    internal void SetOrder(int order)
    {
        Order = order;
    }

    internal Lecture AddLecture(Guid lectureId, string title, LectureType lectureType, string description)
    {
        int lastOrder = _lectures.Count == 0 ? 0 : _lectures.Max(l => l.Order);
        int lectureOrder = lastOrder + GapSize;
        var lecture = new Lecture(lectureId, Id, title, lectureType, lectureOrder, description);
        _lectures.Add(lecture);
        return lecture;
    }

    internal Result<OrderUpdateResult> UpdateLectureOrder(Guid lectureId, Guid? previousLectureId, Guid? nextLectureId)
    {
        var currentLecture = _lectures.FirstOrDefault(l => l.Id == lectureId);
        if (currentLecture is null) return Result<OrderUpdateResult>.Failure(LectureErrors.NotFound);

        // Move to the very start: no previous sibling
        if (previousLectureId is null && nextLectureId is not null)
        {
            var nextLecture = _lectures.FirstOrDefault(l => l.Id == nextLectureId);
            if (nextLecture is null) return Result<OrderUpdateResult>.Failure(LectureErrors.NotFound);

            // nextLecture must be first, otherwise some other lecture already
            // sits before it and the new order below would collide with that lecture's order.
            bool nextIsNotFirst = _lectures.Any(l => l.Id != currentLecture.Id && l.Order < nextLecture.Order);
            if (nextIsNotFirst) return Result<OrderUpdateResult>.Failure(LectureErrors.InvalidNeighbors);

            // Not enough room between position 0 and nextLecture to fit a value in between - rebalance first.
            if (nextLecture.Order <= MinGapBeforeRebalance)
            {
                RebalanceLectures(GapSize);
                nextLecture = _lectures.First(l => l.Id == nextLectureId);
                currentLecture.SetOrder(nextLecture.Order / 2);
                return BuildOrderUpdateResult(currentLecture, rebalanced: true);
            }

            currentLecture.SetOrder(nextLecture.Order / 2);
            return BuildOrderUpdateResult(currentLecture, rebalanced: false);
        }

        // Move to the very end: no next sibling
        if (previousLectureId is not null && nextLectureId is null)
        {
            var previousLecture = _lectures.FirstOrDefault(l => l.Id == previousLectureId);
            if (previousLecture is null) return Result<OrderUpdateResult>.Failure(LectureErrors.NotFound);

            // previousLecture must be last, otherwise some other lecture already
            // sits after it and the new order below would collide with that lecture's order.
            bool previousIsNotLast = _lectures.Any(l => l.Id != currentLecture.Id && l.Order > previousLecture.Order);
            if (previousIsNotLast) return Result<OrderUpdateResult>.Failure(LectureErrors.InvalidNeighbors);

            // Appending past the last lecture always has a full gap of room above it - never rebalances.
            currentLecture.SetOrder(previousLecture.Order + GapSize);
            return BuildOrderUpdateResult(currentLecture, rebalanced: false);
        }

        // Move between two lectures: both neighbors are given.
        if (previousLectureId is not null && nextLectureId is not null)
        {
            var previousLecture = _lectures.FirstOrDefault(l => l.Id == previousLectureId);
            var nextLecture = _lectures.FirstOrDefault(l => l.Id == nextLectureId);
            if (previousLecture is null || nextLecture is null) return Result<OrderUpdateResult>.Failure(LectureErrors.NotFound);

            if (previousLecture.Order >= nextLecture.Order)
                return Result<OrderUpdateResult>.Failure(LectureErrors.InvalidNeighbors);

            // previousLecture and nextLecture must be adjacent siblings, otherwise some
            // other lecture sits between them and the new order below would collide with it.
            bool areAdjacent = !_lectures.Any(l =>
                l.Id != currentLecture.Id && l.Order > previousLecture.Order && l.Order < nextLecture.Order);
            if (!areAdjacent) return Result<OrderUpdateResult>.Failure(LectureErrors.InvalidNeighbors);

            // Not enough room between the two neighbors to fit a value in between - renumber first.
            if (nextLecture.Order - previousLecture.Order <= MinGapBeforeRebalance)
            {
                RebalanceLectures(GapSize);
                previousLecture = _lectures.First(l => l.Id == previousLectureId);
                nextLecture = _lectures.First(l => l.Id == nextLectureId);
                currentLecture.SetOrder((previousLecture.Order + nextLecture.Order) / 2);
                return BuildOrderUpdateResult(currentLecture, rebalanced: true);
            }

            currentLecture.SetOrder((previousLecture.Order + nextLecture.Order) / 2);
            return BuildOrderUpdateResult(currentLecture, rebalanced: false);
        }

        // Neither neighbor given - nothing to position relative to.
        return Result<OrderUpdateResult>.Failure(LectureErrors.InvalidNeighbors);
    }

    private Result<OrderUpdateResult> BuildOrderUpdateResult(Lecture movedLecture, bool rebalanced)
    {
        var snapshot = rebalanced ? _lectures.ToDictionary(l => l.Id, l => l.Order) : null;
        return new OrderUpdateResult(movedLecture.Id, movedLecture.Order, rebalanced, snapshot);
    }

    private void RebalanceLectures(int gapSize)
    {
        var lectures = _lectures.OrderBy(e => e.Order).ToList();
        for (int i = 0; i < lectures.Count; i++)
        {
            lectures[i].SetOrder((i + 1) * gapSize);
        }
    }

    internal Result AddTask(Guid taskId, Guid courseId, string taskType)
    {
        var task = _sectionTasks.FirstOrDefault(t => t.TaskId == taskId);
        if (task is not null) return Result.Failure(SectionErrors.TaskAlreadyAssigned);
        _sectionTasks.Add(new SectionTask(Id, courseId, taskId, taskType));
        return Result.Success();
    }

    internal Result RemoveTask(Guid taskId)
    {
        var task = _sectionTasks.FirstOrDefault(t => t.TaskId == taskId);
        if (task is null) return Result.Failure(SectionErrors.TaskNotFound);
        _sectionTasks.Remove(task);
        return Result.Success();
    }

    internal bool HasTask(Guid taskId) => _sectionTasks.Any(t => t.TaskId == taskId);


}
