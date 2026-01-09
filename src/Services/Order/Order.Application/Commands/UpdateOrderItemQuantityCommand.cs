using MediatR;
using Order.Domain.Repositories;
using Order.Domain.ValueObjects;

namespace Order.Application.Commands;

public record UpdateOrderItemQuantityCommand : IRequest<bool>
{
    public Guid OrderId { get; init; }
    public Guid ItemId { get; init; }
    public int NewQuantity { get; init; }
}

public class UpdateOrderItemQuantityCommandHandler(IOrderRepository orderRepository) : IRequestHandler<UpdateOrderItemQuantityCommand, bool>
{
    public async Task<bool> Handle(UpdateOrderItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(OrderId.From(request.OrderId), cancellationToken);
        if (order == null)
            return false;

        order.UpdateItemQuantity(OrderItemId.From(request.ItemId), request.NewQuantity);

        orderRepository.Update(order);
        await orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return true;
    }
}
