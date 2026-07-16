using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Enums;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Domain.Entities;


public sealed class Section : Entity<Guid>
{
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

    internal Lecture AddLecture(Guid lectureId, string title, LectureType lectureType, string description)
    {
        int lectureOrder = _lectures.Count + 1;
        var lecture = new Lecture(lectureId, Id, title, lectureType, lectureOrder, description);
        _lectures.Add(lecture);
        return lecture;
    }

    internal Result UpdateLectureOrder(Guid lectureId, int order)
    {
        var lecture = _lectures.FirstOrDefault(l => l.Id == lectureId);

        if (lecture is null) return Result.Failure(LectureErrors.NotFound);

        var lectureAtGivenOrder = _lectures.FirstOrDefault(l => l.Order == order);

        if (lectureAtGivenOrder is not null)
        {
            lectureAtGivenOrder.SetOrder(lecture.Order);
        }
        lecture.SetOrder(order);

        return Result.Success();



    }


}
