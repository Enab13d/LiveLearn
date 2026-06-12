using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Application.Dto;

namespace LiveLearn.Identity.Application.Commands;

  public sealed record UpdateUserProfileCommand(
      Guid UserId,
      string DisplayName,
      string? AvatarUrl,
      string? Bio) : ICommand<UserProfileDto>;
