using MediatR;

namespace LiveLearn.BuildingBlocks;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{

}
