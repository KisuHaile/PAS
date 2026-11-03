using MediatR;

namespace Application.Events;

public class EntityCreatedEvent<T> : INotification where T : class
{
    public T Entity { get; }
    public Guid? UserId { get; }

    public EntityCreatedEvent(T entity, Guid? userId)
    {
        Entity = entity;
        UserId = userId;
    }
}