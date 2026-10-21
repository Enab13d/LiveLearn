using LiveLearn.BuildingBlocks;

namespace LiveLearn.Enrollment.Domain.Errors;


public static class EnrollmentErrors
{

    private static readonly string _prefix = nameof(EnrollmentErrors);

    public static Error EnrollmentRevoked => Error.Conflict($"{_prefix}.EnrollmentRevoked", "Enrollment is revoked");
}
