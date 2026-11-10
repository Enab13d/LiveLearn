using FluentValidation;
using LiveLearn.Ordering.Application.Commands;

namespace LiveLearn.Ordering.Application.Validators;


public sealed class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        // Every purchasable course costs more than zero (O4), so a valid total does too.
        RuleFor(x => x.ExpectedTotal).GreaterThan(0);
    }
}
