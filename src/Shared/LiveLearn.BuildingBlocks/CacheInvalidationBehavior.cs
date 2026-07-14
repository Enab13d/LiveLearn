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
            foreach (var key in request.CacheKeys)
                await cache.RemoveAsync(key.ToLower(), ct);

        return result;
    }
}
