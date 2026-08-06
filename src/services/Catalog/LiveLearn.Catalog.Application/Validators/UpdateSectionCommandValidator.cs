using FluentValidation;
using LiveLearn.Catalog.Application.Commands;

namespace LiveLearn.Catalog.Application.Validators;

public sealed class UpdateSectionCommandValidator : AbstractValidator<UpdateSectionCommand>
{
    public UpdateSectionCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(128);
    }
}
