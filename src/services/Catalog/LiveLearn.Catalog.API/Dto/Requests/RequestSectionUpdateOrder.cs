namespace LiveLearn.Catalog.API.Dto.Requests;


public sealed record RequestSectionUpdateOrder(Guid? PreviousSectionId, Guid? NextSectionId);
