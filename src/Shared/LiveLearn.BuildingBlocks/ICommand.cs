using MediatR;

namespace LiveLearn.BuildingBlocks;

public interface ICommand : IRequest<Result>;
public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
