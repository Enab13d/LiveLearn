using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Errors;

public static class CategoryErrors
{
    public static readonly Error NotFound = Error.NotFound("Category.NotFound", "Category was not found");
}
