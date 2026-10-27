using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Application.Dto;
using LiveLearn.Identity.Application.Repositories;
using LiveLearn.Identity.Application.Services;
using LiveLearn.Identity.Domain.Errors;

namespace LiveLearn.Identity.Application.Queries;

internal sealed class GetCurrentUserProfileQueryHandler(ICurrentUser currentUser, IUserRepository userRepository)
    : IQueryHandler<GetCurrentUserProfileQuery, UserProfileDto>
{
    public async Task<Result<UserProfileDto>> Handle(GetCurrentUserProfileQuery query, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(currentUser.UserId, ct);

        if (user is null)
            return Result<UserProfileDto>.Failure(UserErrors.NotFound);

        var dto = new UserProfileDto(
            user.Id,
            user.Email,
            user.DisplayName,
            user.AvatarUrl,
            user.Bio);

        return Result<UserProfileDto>.Success(dto);
    }
}
