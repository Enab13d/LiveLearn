using FluentValidation;
using LiveLearn.Catalog.Application.Commands;

namespace LiveLearn.Catalog.Application.Validators;

public sealed class UpdateSectionOrderCommandValidator : AbstractValidator<UpdateSectionOrderCommand>
{
    public UpdateSectionOrderCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.Order).GreaterThanOrEqualTo(1);
    }
}
