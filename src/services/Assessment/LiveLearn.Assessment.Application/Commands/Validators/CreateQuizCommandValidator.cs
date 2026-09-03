using FluentValidation;

namespace LiveLearn.Assessment.Application.Commands.Validators;


internal sealed class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizCommandValidator()
    {
        RuleFor(x => x.TutorId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.PassingScore).InclusiveBetween(0, 100);
    }

}
