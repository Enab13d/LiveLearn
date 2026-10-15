using LiveLearn.Catalog.Application.Dto;

namespace LiveLearn.Catalog.Application.Services;


public interface IWatchTicketSigner
{
    public WatchTicketDto IssueWatchTicket(Guid userId, Guid lectureId, Guid videoId);
}
