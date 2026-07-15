using System.ComponentModel.DataAnnotations;

namespace LiveLearn.Catalog.API.Dto.Requests;


public sealed record RequestCourseDto
{
    [Required]
    public Guid CategoryId { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public decimal Price { get; set; }
    
};
