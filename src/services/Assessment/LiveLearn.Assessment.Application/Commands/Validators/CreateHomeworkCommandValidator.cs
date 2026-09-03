using FluentValidation;

namespace LiveLearn.Assessment.Application.Commands.Validators;


public sealed class CreateHomeworkCommandValidator : AbstractValidator<CreateHomeworkCommand>
{
    public CreateHomeworkCommandValidator()
    {
        RuleFor(x => x.TutorId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.Description).NotEmpty().MinimumLength(64).MaximumLength(4096);
    }
}
