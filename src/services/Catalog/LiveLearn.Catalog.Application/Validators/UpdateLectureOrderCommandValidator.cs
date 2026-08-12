using FluentValidation;
using LiveLearn.Catalog.Application.Commands;

namespace LiveLearn.Catalog.Application.Validators;

public sealed class UpdateLectureOrderCommandValidator : AbstractValidator<UpdateLectureOrderCommand>
{
    public UpdateLectureOrderCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.SectionId).NotEmpty();
        RuleFor(x => x.LectureId).NotEmpty();
        RuleFor(x => x)
            .Must(x => x.PreviousLectureId is not null || x.NextLectureId is not null)
            .WithMessage("At least one of PreviousLectureId or NextLectureId is required.");
        RuleFor(x => x.PreviousLectureId)
            .NotEqual(x => x.LectureId)
            .When(x => x.PreviousLectureId is not null)
            .WithMessage("PreviousLectureId cannot equal LectureId.");
        RuleFor(x => x.NextLectureId)
            .NotEqual(x => x.LectureId)
            .When(x => x.NextLectureId is not null)
            .WithMessage("NextLectureId cannot equal LectureId.");
        RuleFor(x => x)
            .Must(x => x.PreviousLectureId is null || x.NextLectureId is null || x.PreviousLectureId != x.NextLectureId)
            .WithMessage("PreviousLectureId and NextLectureId cannot be the same.");
    }
}
