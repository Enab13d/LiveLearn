using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Application.Dto;

namespace LiveLearn.Identity.Application.Commands;

  public sealed record UpdateUserProfileCommand(
      string DisplayName,
      string? AvatarUrl,
      string? Bio) : ICommand<UserProfileDto>;
