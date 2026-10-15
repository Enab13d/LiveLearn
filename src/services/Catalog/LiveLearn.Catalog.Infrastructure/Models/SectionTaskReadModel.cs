namespace LiveLearn.Catalog.Infrastructure.Models;

internal class SectionTaskReadModel
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid SectionId { get; set; }
    public Guid CourseId { get; set; }
    public string TaskType { get; set; } = string.Empty;
    public long CreatedRevision { get; set; }
    public long? ArchivedRevision { get; set; }

}
