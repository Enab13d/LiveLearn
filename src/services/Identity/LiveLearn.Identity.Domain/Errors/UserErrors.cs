using LiveLearn.BuildingBlocks;

namespace LiveLearn.Identity.Domain.Errors;

public static class UserErrors
{
    public static readonly Error NotFound = Error.NotFound("User.NotFound", "User was not found.");
    public static readonly Error EmailAlreadyTaken = Error.Conflict("User.EmailAlreadyTaken", "This email is already in  use.");
}
