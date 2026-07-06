using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record UpdateCourseCommand(Guid CourseId, string Title, string Description, decimal Price, Guid CategoryId) : ICommand<CourseDto>;
