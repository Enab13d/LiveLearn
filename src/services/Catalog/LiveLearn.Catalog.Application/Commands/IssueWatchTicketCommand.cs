using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record IssueWatchTicketCommand(Guid CourseId, Guid SectionId, Guid LectureId) : ICommand<WatchTicketDto>;
