using FluentValidation;
using LiveLearn.Ordering.Application.Commands;

namespace LiveLearn.Ordering.Application.Validators;


public sealed class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
    }
}
