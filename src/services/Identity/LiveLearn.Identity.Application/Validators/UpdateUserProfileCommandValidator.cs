using FluentValidation;
using LiveLearn.Identity.Application.Commands;

namespace LiveLearn.Identity.Application.Validators;


public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(user => user.DisplayName)
            .NotEmpty().WithMessage("Display name is required");
    }
};
