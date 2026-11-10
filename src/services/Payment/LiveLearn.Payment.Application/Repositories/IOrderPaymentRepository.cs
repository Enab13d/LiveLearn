using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Domain.Entities;

namespace LiveLearn.Payment.Application.Repositories;


// GetByIdAsync / FindAsync load the payment with its lines.
public interface IOrderPaymentRepository : IRepository<OrderPayment, Guid>;
