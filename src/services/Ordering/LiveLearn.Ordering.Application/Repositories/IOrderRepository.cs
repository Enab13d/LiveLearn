using LiveLearn.BuildingBlocks;
using LiveLearn.Ordering.Domain.Entities;

namespace LiveLearn.Ordering.Application.Repositories;


// GetByIdAsync / FindAsync load the order with its lines.
public interface IOrderRepository : IRepository<Order, Guid>;
