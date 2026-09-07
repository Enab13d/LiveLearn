using LiveLearn.Assessment.Domain.DomainEvents;
using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.Entities;


public abstract class AssessmentTask : AggregateRoot<Guid>
{

    protected AssessmentTask() { }
    public string Title { get; protected set; } = string.Empty;
    public Guid TutorId { get; protected set; }
    public Guid? SectionId { get; protected set; }
    public Guid? CourseId { get; protected set; }

    public void MarkAsDeleted() => RaiseDomainEvent(new TaskDeletedDomainEvent(Id));

    public void AssignToCourseAndSection(Guid courseId, Guid sectionId)
    {
        CourseId = courseId;
        SectionId = sectionId;
    }
}
