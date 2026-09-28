namespace LiveLearn.Identity.Application.Dto;

public record struct UserProfileDto
(
    Guid Id,
    string Email,
    string DisplayName,
    string AvatarUrl,
    string Bio
);
