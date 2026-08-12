using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Domain.Common;

namespace LiveLearn.Catalog.Application.Commands;

public sealed record UpdateSectionOrderCommand(
    Guid CourseId,
    Guid TutorId,
    Guid SectionId,
    Guid? PreviousSectionId,
    Guid? NextSectionId) : ICommand<OrderUpdateResult>;
