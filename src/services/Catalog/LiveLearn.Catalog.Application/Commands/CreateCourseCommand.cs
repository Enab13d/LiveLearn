using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record CreateCourseCommand(Guid categoryId, string title, string description, decimal price) : ICommand<CourseDto>;
