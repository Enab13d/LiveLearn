using FluentValidation;

namespace LiveLearn.Assessment.Application.Commands.Validators;


public sealed class ArchiveTaskCommandValidator : AbstractValidator<ArchiveTaskCommand>
{
    public ArchiveTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.TutorId).NotEmpty();
    }
}
