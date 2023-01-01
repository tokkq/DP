using System;
using System.Collections.Generic;
using DailyProject_221204.Scripts;

namespace DailyProject_221204
{
    public class EventPublisher
    {
        event Action? _event;

        List<Action> _subscribedEvent = new List<Action>();

        public void Publish()
        {
            _event?.Invoke();
        }

        public IDisposable Subscribe(Action action)
        {
            _event += action;
            _subscribedEvent.Add(action);
            return new ActionDisposer(() =>
            {
                _event -= action;
                _subscribedEvent.Remove(action);
            });
        }

        public void Clear()
        {
            foreach (var i in _subscribedEvent)
            {
                _event -= i;
            }
            _subscribedEvent.Clear();
        }
    }

    public class EventPublisher<T1>
    {
        event Action<T1>? _event;

        List<Action<T1>> _subscribedEvent = new List<Action<T1>>();

        public void Publish(T1 t1)
        {
            _event?.Invoke(t1);
        }

        public IDisposable Subscribe(Action<T1> action)
        {
            _event += action;
            _subscribedEvent.Add(action);
            return new ActionDisposer(() =>
            {
                _event -= action;
                _subscribedEvent.Remove(action);
            });
        }

        public void Clear()
        {
            foreach (var i in _subscribedEvent)
            {
                _event -= i;
            }
            _subscribedEvent.Clear();
        }
    }
}
