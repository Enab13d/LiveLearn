using FluentValidation;

namespace LiveLearn.Assessment.Application.Commands.Validators;


public sealed class ReviewHomeworkSumbissionCommandValidator : AbstractValidator<ReviewHomeworkSubmissionCommand>
{
    public ReviewHomeworkSumbissionCommandValidator()
    {
        RuleFor(x => x.TutorId).NotEmpty();
        RuleFor(x => x.Feedback).NotEmpty().MinimumLength(8).MaximumLength(2048);
        RuleFor(x => x.SubmissionId).NotEmpty();


    }
}
