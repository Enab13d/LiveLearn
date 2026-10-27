using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Application.Repositories;
using LiveLearn.Identity.Application.Services;
using LiveLearn.Identity.Domain.Entities;

namespace LiveLearn.Identity.Application.Commands;

internal sealed class ProvisionUserCommandHandler(ICurrentUser currentUser, IUserRepository userRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<ProvisionUserCommand>
{
    public async Task<Result> Handle(ProvisionUserCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(currentUser.UserId, ct);

        if (user is not null && user.Email == currentUser.Email)
            return Result.Success(); // already in sync, nothing to do

        if (user is null)
        {
            user = User.Create(currentUser.UserId, currentUser.FirstName, currentUser.LastName, currentUser.Email);
            await userRepository.AddAsync(user, ct);
        }
        else
        {
            user.UpdateAuthData(currentUser.Email);
        }

        await unitOfWork.CommitAsync(ct);
        return Result.Success();
    }
}
