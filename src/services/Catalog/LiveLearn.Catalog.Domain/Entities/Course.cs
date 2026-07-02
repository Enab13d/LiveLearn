using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Catalog.Domain.Enums;
using LiveLearn.Catalog.Domain.Errors;

namespace LiveLearn.Catalog.Domain.Entities;


public sealed class Course : AggregateRoot<Guid>
{
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

    public Result Publish()
    {
        if (Status != CourseStatus.Draft) return Result.Failure(CourseErrors.PublishFailed);

        Status = CourseStatus.Published;
        RaiseDomainEvent(new CoursePublishedDomainEvent(Id));
        return Result.Success();
    }

    public Result AddLecture(Guid sectionId, Guid lectureId, string title, LectureType lectureType, int order, TimeSpan duration)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);
        var lecture = section.AddLecture(lectureId, title, lectureType, order, duration);
        RaiseDomainEvent(new LectureAddedDomainEvent(Id, sectionId, lecture.Id));
        return Result.Success();
    }



}
