using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.DomainEvents;
using LiveLearn.Catalog.Domain.Enums;

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



}
