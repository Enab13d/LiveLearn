using FluentValidation;
using LiveLearn.Catalog.Application.Commands;

namespace LiveLearn.Catalog.Application.Validators;

public sealed class UpdateSectionOrderCommandValidator : AbstractValidator<UpdateSectionOrderCommand>
{
    public UpdateSectionOrderCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x)
            .Must(x => x.PreviousSectionId is not null || x.NextSectionId is not null)
            .WithMessage("At least one of PreviousSectionId or NextSectionId is required.");
        RuleFor(x => x.PreviousSectionId)
            .NotEqual(x => x.SectionId)
            .When(x => x.PreviousSectionId is not null)
            .WithMessage("PreviousSectionId cannot equal SectionId.");
        RuleFor(x => x.NextSectionId)
            .NotEqual(x => x.SectionId)
            .When(x => x.NextSectionId is not null)
            .WithMessage("NextSectionId cannot equal SectionId.");
        RuleFor(x => x)
            .Must(x => x.PreviousSectionId is null || x.NextSectionId is null || x.PreviousSectionId != x.NextSectionId)
            .WithMessage("PreviousSectionId and NextSectionId cannot be the same.");
    }
}
