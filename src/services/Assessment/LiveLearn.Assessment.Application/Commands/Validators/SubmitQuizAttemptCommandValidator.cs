using FluentValidation;

namespace LiveLearn.Assessment.Application.Commands.Validators;


public sealed class SubmitQuizAttemptCommandValidator : AbstractValidator<SubmitQuizAttemptCommand>
{
    public SubmitQuizAttemptCommandValidator()
    {
        RuleFor(x => x.QuizId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Answers).NotEmpty();
        RuleForEach(x => x.Answers).Must(kvp => kvp.Key != Guid.Empty && kvp.Value != Guid.Empty);
    }
}
