using LiveLearn.BuildingBlocks;
using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Commands;


public sealed record IssueWatchTicketCommand(Guid UserId, Guid CourseId, Guid SectionId, Guid LectureId, bool IsAdmin) : ICommand<WatchTicketDto>;
