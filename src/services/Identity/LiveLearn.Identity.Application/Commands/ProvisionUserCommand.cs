

using LiveLearn.BuildingBlocks;

namespace LiveLearn.Identity.Application.Commands;

public sealed record ProvisionUserCommand(Guid Id, string Email, string FirstName, string LastName) : ICommand;
