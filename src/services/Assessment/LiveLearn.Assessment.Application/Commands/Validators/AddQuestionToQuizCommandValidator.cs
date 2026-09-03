using FluentValidation;

namespace LiveLearn.Assessment.Application.Commands.Validators;

public sealed class AddQuestionToQuizCommandValidator : AbstractValidator<AddQuestionToQuizCommand>
{
    public AddQuestionToQuizCommandValidator()
    {
        RuleFor(x => x.TutorId).NotEmpty();
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty().MinimumLength(8).MaximumLength(256);
        RuleFor(x => x.Answers).NotEmpty().Must(l => l.Count >= 2 && l.Count <= 6);
        RuleFor(x => x.CorrectAnswerIdx)
            .Must((model, currentValue) => 
                currentValue < model.Answers.Count && currentValue >= 0);
    }
}
