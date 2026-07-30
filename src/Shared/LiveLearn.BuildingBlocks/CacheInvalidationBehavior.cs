using MediatR;

namespace LiveLearn.BuildingBlocks;

public sealed class CacheInvalidationBehavior<TRequest, TResponse>(ICacheService cache): IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>, ICacheInvalidationCommand
where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var result = await next(ct);
        if (result.IsSuccess)
            foreach (var tag in request.Tags)
                await cache.RemoveByTagAsync(tag, ct);

        return result;
    }
}
