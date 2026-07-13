namespace LiveLearn.Catalog.Infrastructure.Models;

internal class CourseReadModel
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public Guid TutorId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Status { get; set; } = string.Empty;

    public CategoryReadModel Category { get; set; } = default!;

    public List<SectionReadModel> Sections { get; set; } = [];
}
