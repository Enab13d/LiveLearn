using LiveLearn.BuildingBlocks;
using LiveLearn.Payment.Domain.Entities;

namespace LiveLearn.Payment.Application.Repositories;


// Every lookup loads the payment with its lines.
public interface IOrderPaymentRepository : IRepository<OrderPayment, Guid>
{
    // OrderId is unique (D13); every message and webhook identifies the payment by it.
    Task<OrderPayment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}
