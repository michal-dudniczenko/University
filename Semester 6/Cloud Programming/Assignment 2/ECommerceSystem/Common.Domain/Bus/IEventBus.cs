using Common.Domain.Events;

namespace Common.Domain.Bus;

public interface IEventBus
{
    Task Publish<T>(T @event) where T : Event;

    Task Subscribe<T, TH, TS>() where T : Event where TH : IEventHandler<T> where TS : Service;
}
