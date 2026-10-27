using LiveLearn.BuildingBlocks;

namespace LiveLearn.Enrollment.Domain.Errors;


public static class EnrollmentErrors
{

    private static readonly string _prefix = nameof(EnrollmentErrors);

    public static Error NotFound => Error.NotFound($"{_prefix}.NotFound", "Enrollment was not found");

    public static Error EnrollmentRevoked => Error.Conflict($"{_prefix}.EnrollmentRevoked", "Enrollment is revoked");

    public static Error CourseStructureUnavailable => Error.Conflict($"{_prefix}.CourseStructureUnavailable", "Course structure is not available yet");
}
