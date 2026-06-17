

using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Domain.Entities;

namespace LiveLearn.Identity.Application.Commands;

public sealed record ProvisionUserCommand(Guid Id, string Email, string FirstName, string LastName, Role UserRole) : ICommand;
