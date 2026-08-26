using LiveLearn.BuildingBlocks;

namespace LiveLearn.Assessment.Domain.Entities;


public abstract class AssessmentTask : AggregateRoot<Guid>
{

    protected AssessmentTask() { }
    public string Title { get; private set; } = string.Empty;
    public Guid TutorId { get; private set; }
    public Guid SectionId { get; private set; }
    public Guid CourseId { get; private set; }
}
