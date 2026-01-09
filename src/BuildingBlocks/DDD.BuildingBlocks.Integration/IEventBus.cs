namespace BuildingBlocks.Integration;

/// <summary>
/// Interface for the Event Bus.
/// The Event Bus is responsible for publishing Integration Events between Bounded Contexts.
/// This is part of the Anti-Corruption Layer pattern in DDD.
/// </summary>
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IntegrationEvent;

    void Subscribe<TEvent, THandler>()
        where TEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TEvent>;
}

public interface IIntegrationEventHandler<in TEvent>
    where TEvent : IntegrationEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
