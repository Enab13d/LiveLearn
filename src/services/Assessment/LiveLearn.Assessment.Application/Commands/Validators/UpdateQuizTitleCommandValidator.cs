using FluentValidation;

namespace LiveLearn.Assessment.Application.Commands.Validators;


public sealed class UpdateQuizTitleCommandValidator : AbstractValidator<UpdateQuizTitleCommand>
{

    public UpdateQuizTitleCommandValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.TutorId).NotEmpty();
        RuleFor(x => x.NewTitle).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}
