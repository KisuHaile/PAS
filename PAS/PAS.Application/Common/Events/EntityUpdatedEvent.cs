using MediatR;

namespace Application.Events;

public class EntityUpdatedEvent<T> : INotification where T : class
{
    public T Entity { get; }
    public T? OldEntity { get; }
    public Guid? UserId { get; }

    public EntityUpdatedEvent(T entity, Guid? userId)
    {
        Entity = entity;
        UserId = userId;
    }

    public EntityUpdatedEvent(T entity, T oldEntity, Guid? userId)
    {
        Entity = entity;
        OldEntity = oldEntity;
        UserId = userId;
    }
}