using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Application.Repositories;
using LiveLearn.Identity.Domain.Entities;

namespace LiveLearn.Identity.Application.Commands;

internal sealed class ProvisionUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<ProvisionUserCommand>
{
    public async Task<Result> Handle(ProvisionUserCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.Id, ct);
        if (user is not null)
            return Result.Success();

        user = User.Create(request.Id, request.FirstName, request.LastName, request.Email, request.UserRole);
        await userRepository.AddAsync(user, ct);
        await unitOfWork.CommitAsync(ct);
        return Result.Success();
    }
}
