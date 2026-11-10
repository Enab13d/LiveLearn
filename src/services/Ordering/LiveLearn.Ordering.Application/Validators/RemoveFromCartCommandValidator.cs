using FluentValidation;
using LiveLearn.Ordering.Application.Commands;

namespace LiveLearn.Ordering.Application.Validators;


public sealed class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
{
    public RemoveFromCartCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
    }
}
