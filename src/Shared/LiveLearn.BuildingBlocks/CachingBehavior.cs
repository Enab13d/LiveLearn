using MediatR;

namespace LiveLearn.BuildingBlocks;

public sealed class CachingBehavior<TRequest, TResponse>(ICacheService cacheService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheableQuery
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        TResponse response;

        if (request.BypassCache) return await next(cancellationToken);

        var cachedResponse = await cacheService.GetAsync<TResponse>(request.CacheKey.ToLower(), cancellationToken);
        if (cachedResponse is not null)
        {
            response = cachedResponse;
        }
        else
        {
            response = await next(cancellationToken);
            if (response.IsSuccess)
                await cacheService.SetAsync(request.CacheKey.ToLower(), response, request.Options, cancellationToken);
        }

        return response;
    }
}
