using FluentValidation;
using LiveLearn.Catalog.Application.Queries;

namespace LiveLearn.Catalog.Application.Validators;


public sealed class GetMyCoursesQueryValidator : AbstractValidator<GetMyCoursesQuery>
{
    public GetMyCoursesQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
