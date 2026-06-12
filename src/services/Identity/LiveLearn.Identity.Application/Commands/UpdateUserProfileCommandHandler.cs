using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Application.Dto;
using LiveLearn.Identity.Application.Repositories;
using LiveLearn.Identity.Domain.Errors;

namespace LiveLearn.Identity.Application.Commands;


internal sealed class UpdateUserProfileCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
 : ICommandHandler<UpdateUserProfileCommand, UserProfileDto>
{
    public async Task<Result<UserProfileDto>> Handle(UpdateUserProfileCommand request, CancellationToken ct)
    {

        var user = await userRepository.GetByIdAsync(request.UserId, ct);

        if (user is null)
            return Result<UserProfileDto>.Failure(UserErrors.NotFound);

        user.UpdateProfile(request.DisplayName, request.AvatarUrl, request.Bio);
        userRepository.Update(user);
        await unitOfWork.CommitAsync(ct);
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
