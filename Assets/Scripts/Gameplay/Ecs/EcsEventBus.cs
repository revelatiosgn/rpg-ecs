using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Gameplay.Ecs
{
    public class EcsEventBus
    {
        private int _systemsCount;

        public EcsEventBus(int systemsCount)
        {
            _systemsCount = systemsCount;
            _pools = new Dictionary<Type, IEcsEventPool>(_systemsCount);
        }

        private Dictionary<Type, IEcsEventPool> _pools;

        public void RaiseEvent<T>(T eventBody) where T : struct
        {
            GetPool<T>().Add(eventBody);
        }

        public void Swallow()
        {
            foreach (var pool in _pools)
                pool.Value.Swallow();
        }

        public void Destroy()
        {
            _pools.Clear();
        }

        public EcsEventPool<T> GetPool<T>() where T : struct
        {
            if (_pools.TryGetValue(typeof(T), out IEcsEventPool ipool))
                return (EcsEventPool<T>) ipool;

            EcsEventPool<T> pool = new EcsEventPool<T>(_systemsCount);
            _pools[typeof(T)] = pool;

            return pool;
        }
    }

    public interface IEcsEventPool
    {
        void Swallow();
    }

    public class EcsEventPool<T> : IEcsEventPool, IEnumerable<T> where T : struct
    {
        internal const int DefaultPoolSize = 256;

        private Queue<int> _eventsQueue;

        private EventItem<T>[] _events;
        private int _eventsCount;

        private int[] _recycledEvents;
        private int _recycledEventsCount;

        private int _systemsCount;

        public EcsEventPool(int systemsCount)
        {
            _systemsCount = systemsCount;
            
            _eventsQueue = new Queue<int>(DefaultPoolSize);
            _events = new EventItem<T>[DefaultPoolSize];
            _eventsCount = 0;
            _recycledEvents = new int[DefaultPoolSize];
            _recycledEventsCount = 0;
        }

        public void Add(T eventBody)
        {
            if (_recycledEventsCount > 0)
            {
                int eventIndex = _recycledEvents[--_recycledEventsCount];
                _eventsQueue.Enqueue(eventIndex);
                _events[eventIndex] = new EventItem<T> { Body = eventBody, Counter = _systemsCount };
            }
            else
            {
                if (_eventsQueue.Count == _events.Length)
                    Array.Resize(ref _events, _events.Length << 1);

                int eventIndex = _eventsCount++;
                _eventsQueue.Enqueue(eventIndex);
                _events[eventIndex] = new EventItem<T> { Body = eventBody, Counter = _systemsCount };
            }
        }

        public void Swallow()
        {
            foreach (int q in _eventsQueue)
            {
                _events[q].Counter--;
            }

            while (_eventsQueue.Count > 0)
            {
                if (_events[_eventsQueue.Peek()].Counter == 0)
                {
                    int eventIndex = _eventsQueue.Dequeue();
                    _events[eventIndex].Body = default;
                    
                    if (_recycledEventsCount == _recycledEvents.Length)
                        Array.Resize(ref _recycledEvents, _recycledEvents.Length << 1);

                    _recycledEvents[++_recycledEventsCount] = eventIndex;
                }
                else
                {
                    break;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (int q in _eventsQueue)
            {
                yield return _events[q].Body;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private struct EventItem<V> where V : struct
        {
            public V Body;
            public int Counter;
        }

        public override string ToString()
        {
            string result = "";
            
            foreach (int q in _eventsQueue)
            {
                result += $"{_events[q].Counter} {_events[q].Body.ToString()} / ";
            }

            return result;
        }
    }
}


