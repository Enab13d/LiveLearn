using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Services;

namespace LiveLearn.Ordering.Application.Commands;


internal sealed class ClearCartCommandHandler(ICurrentUser currentUser, ICartStore cartStore)
    : ICommandHandler<ClearCartCommand>
{
    public async Task<Result> Handle(ClearCartCommand request, CancellationToken ct)
    {
        await cartStore.ClearAsync(currentUser.UserId, ct);

        return Result.Success();
    }
}
