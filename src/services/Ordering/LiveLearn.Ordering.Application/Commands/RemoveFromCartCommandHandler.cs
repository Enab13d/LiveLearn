using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Application.Services;

namespace LiveLearn.Ordering.Application.Commands;


internal sealed class RemoveFromCartCommandHandler(ICurrentUser currentUser, ICartStore cartStore)
    : ICommandHandler<RemoveFromCartCommand>
{
    public async Task<Result> Handle(RemoveFromCartCommand request, CancellationToken ct)
    {
        await cartStore.RemoveAsync(currentUser.UserId, [request.CourseId], ct);

        return Result.Success();
    }
}
