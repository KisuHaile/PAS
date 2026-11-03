using MediatR;

namespace Application.Events;

public class EntityDeletedEvent<T> : INotification where T : class
{
    public T Entity { get; }
    public Guid? UserId { get; }

    public EntityDeletedEvent(T entity, Guid? userId)
    {
        Entity = entity;
        UserId = userId;
    }
}