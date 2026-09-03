using FluentValidation;

namespace LiveLearn.Assessment.Application.Commands.Validators;


public sealed class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    public DeleteTaskCommandValidator()
    {
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.TutorId).NotEmpty();
    }
}
