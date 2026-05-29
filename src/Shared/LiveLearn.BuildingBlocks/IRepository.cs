namespace LiveLearn.BuildingBlocks;

public interface IRepository<TEntity, TId> where TEntity : Entity<TId>
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken ct = default);

    Task<IEnumerable<TEntity>> FindAsync(
        System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate,
        CancellationToken ct = default);

    Task AddAsync(TEntity entity, CancellationToken ct = default);

    void Update(TEntity entity);

    void Delete(TEntity entity);
}
