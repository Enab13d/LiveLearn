using FluentValidation;
using LiveLearn.Catalog.Application.Queries;

namespace LiveLearn.Catalog.Application.Validators;


public sealed class GetCatalogQueryValidator : AbstractValidator<GetCatalogQuery>
{
    public GetCatalogQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Query).MinimumLength(2).When(x => x.Query is not null);
        RuleFor(x => x.MaxPrice).GreaterThan(0).When(x => x.MaxPrice is not null);
    }
}
