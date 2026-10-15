namespace LiveLearn.Catalog.Application.Dto;

public sealed record WatchTicketDto(string Ticket, Guid VideoId, DateTimeOffset ExpiresAt);
