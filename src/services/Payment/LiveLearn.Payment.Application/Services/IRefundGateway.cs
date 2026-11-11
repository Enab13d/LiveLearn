using LiveLearn.BuildingBlocks;

namespace LiveLearn.Payment.Application.Services;


// Partial refunds of an order's charge at the PSP (D6, D9). No PSP types cross this port.
public interface IRefundGateway
{
    // Creates the refund once per idempotency key (OrderLineId); a retry with the same request returns the
    // same refund. A permanent rejection is a failure; transient errors throw so the caller's retry runs
    // again (D14). Returns the refund id.
    Task<Result<string>> CreateRefundAsync(RefundRequest request, string idempotencyKey, CancellationToken ct);

    // Re-reads the refund for a webhook (D14, D15). A refund that isn't ours (no OrderId / OrderLineId in
    // its metadata) or doesn't exist is a failure.
    Task<Result<RefundState>> GetRefundAsync(string refundId, CancellationToken ct);
}
