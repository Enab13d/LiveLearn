using LiveLearn.BuildingBlocks;
using LiveLearn.Identity.Domain.Entities;

namespace LiveLearn.Identity.Application.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
}
