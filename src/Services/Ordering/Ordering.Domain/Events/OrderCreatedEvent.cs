using Ordering.Domain.ValueObjects;
namespace Ordering.Domain.Events;

public record OrderCreatedEvent(Order order) : IDomainEvent;
