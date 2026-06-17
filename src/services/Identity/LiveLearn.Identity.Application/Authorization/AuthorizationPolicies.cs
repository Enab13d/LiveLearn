namespace LiveLearn.Identity.Application.Authorization;


public static class AuthorizationPolicies
{
    public static string AdminPolicy => nameof(AdminPolicy);

    public static string TutorPolicy => nameof(TutorPolicy);

    public static string StudentPolicy => nameof(StudentPolicy);
}
