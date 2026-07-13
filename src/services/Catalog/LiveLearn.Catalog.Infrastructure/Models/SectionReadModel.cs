namespace LiveLearn.Catalog.Infrastructure.Models;


internal class SectionReadModel
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<LectureReadModel> Lectures { get; set; } = [];

}
