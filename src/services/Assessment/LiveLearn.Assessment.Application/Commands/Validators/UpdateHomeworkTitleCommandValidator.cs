using FluentValidation;

namespace LiveLearn.Assessment.Application.Commands.Validators;


public sealed class UpdateHomeworkTitleCommandValidator : AbstractValidator<UpdateHomeworkTitleCommand>
{
    public UpdateHomeworkTitleCommandValidator()
    {
        RuleFor(x => x.HomeworkId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(8).MaximumLength(128);
    }
}
