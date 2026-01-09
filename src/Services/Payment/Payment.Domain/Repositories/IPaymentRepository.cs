using BuildingBlocks.Domain;
using Payment.Domain.Aggregates.PaymentAggregate;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.Repositories;

/// <summary>
/// Repository for Payment Aggregate Root.
/// </summary>
public interface IPaymentRepository : IRepository<Aggregates.PaymentAggregate.Payment, PaymentId>
{
    Task<Aggregates.PaymentAggregate.Payment?> GetByOrderIdAsync(
        OrderReference orderId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Aggregates.PaymentAggregate.Payment>> GetByCustomerIdAsync(
        CustomerReference customerId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Aggregates.PaymentAggregate.Payment>> GetByStatusAsync(
        PaymentStatus status,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Aggregates.PaymentAggregate.Payment>> GetPendingPaymentsAsync(
        CancellationToken cancellationToken = default);
}
