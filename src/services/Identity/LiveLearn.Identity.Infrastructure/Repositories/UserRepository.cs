using System.Linq.Expressions;
using LiveLearn.Identity.Application.Repositories;
using LiveLearn.Identity.Domain.Entities;
using LiveLearn.Identity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace LiveLearn.Identity.Infrastructure.Repositories;


internal sealed class UserRepository(IdentityDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User entity, CancellationToken ct = default)
    {
        await dbContext.Users.AddAsync(entity, ct);
    }

    public void Delete(User entity) => dbContext.Users.Remove(entity);

    public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate, CancellationToken ct = default) =>
        await dbContext.Users.Where(predicate).ToListAsync(ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await dbContext.Users.FindAsync([id], ct);

    public void Update(User entity)
    {
        dbContext.Users.Update(entity);
    }
}

