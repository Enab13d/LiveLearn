using FluentValidation;

namespace LiveLearn.Assessment.Application.Commands.Validators;


public sealed class UpdateHomeworkDescriptionCommandValidator : AbstractValidator<UpdateHomeworkDescriptionCommand>
{
    public UpdateHomeworkDescriptionCommandValidator()
    {
        RuleFor(x => x.HomeworkId).NotEmpty();
        RuleFor(x => x.TutorId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MinimumLength(64).MaximumLength(4096);
    }
}
