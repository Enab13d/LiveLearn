using FluentValidation;

namespace LiveLearn.Assessment.Application.Commands.Validators;


public sealed class SubmitHomeworkCommandValidator : AbstractValidator<SubmitHomeworkCommand>
{
    public SubmitHomeworkCommandValidator()
    {
        RuleFor(x => x.HomeworkId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out var parsed))
            .WithMessage(x => $"Property {nameof(x.Content)} must be a valid URI");
    }
}
