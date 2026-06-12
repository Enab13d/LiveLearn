using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Application.Dto;
using LiveLearn.Identity.Application.Repositories;
using LiveLearn.Identity.Domain.Errors;

namespace LiveLearn.Identity.Application.Queries;

internal sealed class GetUserProfileQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserProfileQuery, UserProfileDto>
{
    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery query, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(query.Id, ct);

        if (user is null)
            return Result<UserProfileDto>.Failure(UserErrors.NotFound);

        var dto = new UserProfileDto(
            user.Id,
            user.Email,
            user.DisplayName,
            user.AvatarUrl,
            user.Bio,
            user.Role);

        return Result<UserProfileDto>.Success(dto);
    }
}
