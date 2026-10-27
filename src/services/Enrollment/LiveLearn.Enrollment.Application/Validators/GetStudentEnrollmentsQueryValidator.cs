using FluentValidation;
using LiveLearn.Enrollment.Application.Queries;

namespace LiveLearn.Enrollment.Application.Validators;

public sealed class GetStudentEnrollmentsQueryValidator : AbstractValidator<GetStudentEnrollmentsQuery>
{
    public GetStudentEnrollmentsQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
