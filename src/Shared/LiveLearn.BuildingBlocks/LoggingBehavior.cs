using MediatR;
using Microsoft.Extensions.Logging;

namespace LiveLearn.BuildingBlocks;


public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling Request: {RequestName} {@RequestData}", typeof(TRequest).Name, request);
        var response = await next(cancellationToken);
        if (response.IsSuccess)
        {
            _logger.LogInformation("Handled request: {RequestName} successfully", typeof(TRequest).Name);
        }
        else
        {
            _logger.LogWarning("Handled request: {RequestName} with errors {Errors}", typeof(TRequest).Name, string.Join(',', response.Errors ?? []));
        }

        return response;
    }
}
