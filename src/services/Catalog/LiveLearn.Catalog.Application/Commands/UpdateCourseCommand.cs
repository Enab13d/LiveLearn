using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record UpdateCourseCommand(Guid CourseId, Guid CategoryId, Guid TutorId, string Title, string Description, decimal Price) : ICommand<CourseDto>;
