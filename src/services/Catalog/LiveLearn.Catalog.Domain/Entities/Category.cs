using LiveLearn.BuildingBlocks;

namespace LiveLearn.Catalog.Domain.Entities;

public sealed class Category : AggregateRoot<Guid>
{
    private Category() { }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public static Category Create(Guid id, string name, string slug)
    {
        var category = new Category()
        {
            Id = id,
            Name = name,
            Slug = slug
        };

        return category;
    }

}

