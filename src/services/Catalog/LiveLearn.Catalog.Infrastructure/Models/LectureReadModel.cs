namespace LiveLearn.Catalog.Infrastructure.Models;

internal class LectureReadModel
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public int Order { get; set; }

    public int DurationInSeconds { get; set; }

    public string Description { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

}
