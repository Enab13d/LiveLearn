

using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Domain.Entities;

namespace LiveLearn.Identity.Application.Commands;

public sealed record ProvisionUserCommand(string Email, string FirstName, string LastName, Role UserRole) : ICommand;
