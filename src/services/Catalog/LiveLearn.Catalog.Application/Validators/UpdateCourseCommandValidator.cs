using FluentValidation;
using LiveLearn.Catalog.Application.Commands;
using LiveLearn.Catalog.Application.Repositories;


namespace LiveLearn.Catalog.Application.Validators;

public sealed class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator(ICategoryRepository categoryRepository)
    {

        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(128);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2048);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(course => course.CategoryId)
            .MustAsync(async (id, ct) => await categoryRepository.ExistsAsync(id, ct))
            .WithMessage("Category does not exist");
    }
}
