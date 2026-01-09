using MediatR;
using Order.Domain.Repositories;
using Order.Domain.ValueObjects;

namespace Order.Application.Commands;

public record AddOrderItemCommand : IRequest<bool>
{
    public Guid OrderId { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
}

public class AddOrderItemCommandHandler(IOrderRepository orderRepository) : IRequestHandler<AddOrderItemCommand, bool>
{
    public async Task<bool> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(OrderId.From(request.OrderId), cancellationToken);
        if (order == null)
            return false;

        order.AddItem(
            ProductId.From(request.ProductId),
            request.ProductName,
            Money.FromDecimal(request.UnitPrice),
            request.Quantity);

        orderRepository.Update(order);
        await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return true;
    }
}
