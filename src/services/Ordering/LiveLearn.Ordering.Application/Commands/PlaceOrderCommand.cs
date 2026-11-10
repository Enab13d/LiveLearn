using LiveLearn.BuildingBlocks;

namespace LiveLearn.Ordering.Application.Commands;


// Orders every available cart item at the total the student confirmed (O5). Returns the new order's id.
public sealed record PlaceOrderCommand(decimal ExpectedTotal) : ICommand<Guid>;
