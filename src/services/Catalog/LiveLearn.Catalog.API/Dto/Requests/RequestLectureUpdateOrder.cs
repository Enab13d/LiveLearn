namespace LiveLearn.Catalog.API.Dto.Requests;


public sealed record RequestLectureUpdateOrder(Guid? PreviousLectureId, Guid? NextLectureId);
