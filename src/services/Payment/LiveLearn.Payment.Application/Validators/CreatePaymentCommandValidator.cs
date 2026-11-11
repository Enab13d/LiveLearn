using FluentValidation;
using LiveLearn.Payment.Application.Commands;

namespace LiveLearn.Payment.Application.Validators;


// Shape only; prices and currency are the aggregate's invariants (D8, D13).
public sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.OrderLineId).NotEmpty();
            line.RuleFor(l => l.CourseId).NotEmpty();
            line.RuleFor(l => l.TutorId).NotEmpty();
            // Matches the column; a longer title would fail the insert and be retried forever.
            line.RuleFor(l => l.CourseTitle).NotEmpty().MaximumLength(128);
            line.RuleFor(l => l.Currency).NotEmpty();
        });
    }
}
