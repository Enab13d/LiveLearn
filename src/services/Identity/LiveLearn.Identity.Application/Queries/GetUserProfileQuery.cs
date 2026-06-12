using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Application.Dto;

namespace LiveLearn.Identity.Application.Queries;

public sealed record GetUserProfileQuery(Guid Id) : IQuery<UserProfileDto>;
