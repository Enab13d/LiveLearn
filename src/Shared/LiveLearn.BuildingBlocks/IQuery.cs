using MediatR;

namespace LiveLearn.BuildingBlocks;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{

}
