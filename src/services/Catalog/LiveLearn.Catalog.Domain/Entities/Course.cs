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

    public void Update(string title, string description, decimal price, Guid categoryId)
    {
        Title = title;
        Description = description;
        Price = price;
        CategoryId = categoryId;
    }

    public Result Publish()
    {
        if (Status != CourseStatus.Draft) return Result.Failure(CourseErrors.PublishFailed);

        Status = CourseStatus.Published;
        RaiseDomainEvent(new CoursePublishedDomainEvent(Id));
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
        int order = _sections.Count + 1;
        Section section = new(sectionId, Id, title, order);
        _sections.Add(section);
        RaiseDomainEvent(new SectionAddedDomainEvent(sectionId, Id));
        return Result.Success();
    }

    public Result UpdateLectureOrder(Guid sectionId, Guid lectureId, int order)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);

        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);

        var result = section.UpdateLectureOrder(lectureId, order);

        return result;

    }

    public Result UpdateSection(Guid sectionId, string title)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);

        section.Update(title);
        RaiseDomainEvent(new SectionUpdatedDomainEvent(Id, sectionId));
        return Result.Success();
    }

    public Result UpdateSectionOrder(Guid sectionId, int order)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);

        var sectionAtGivenOrder = _sections.FirstOrDefault(s => s.Order == order);
        if (sectionAtGivenOrder is not null)
        {
            sectionAtGivenOrder.SetOrder(section.Order);
        }
        section.SetOrder(order);

        return Result.Success();
    }

    public Result DeleteSection(Guid sectionId)
    {
        var section = _sections.FirstOrDefault(s => s.Id == sectionId);
        if (section is null) return Result.Failure(CourseErrors.SectionNotFound);

        _sections.Remove(section);

        foreach (var laterSection in _sections.Where(s => s.Order > section.Order))
        {
            laterSection.SetOrder(laterSection.Order - 1);
        }

        RaiseDomainEvent(new SectionRemovedDomainEvent(Id, sectionId));
        return Result.Success();
    }

}
