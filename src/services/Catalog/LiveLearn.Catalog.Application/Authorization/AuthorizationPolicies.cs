namespace LiveLearn.Catalog.Application.Authorization;


public static class AuthorizationPolicies
{
    public const string AdminPolicy = nameof(AdminPolicy);

    public const string TutorPolicy = nameof(TutorPolicy);

    public const string StudentPolicy = nameof(StudentPolicy);
}
