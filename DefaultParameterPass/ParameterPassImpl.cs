using System;
using System.Collections.Generic;
using ParameterDeclare;

namespace DefaultParameterPass
{
    public class ParameterPassImpl : IParameterPass
    {
        private readonly Dictionary<int, object> _parameter;
        private readonly Dictionary<int, List<Action<int>>> _observe;
        public ParameterPassImpl()
        {
            _parameter = new Dictionary<int, object>();
            _observe = new Dictionary<int, List<Action<int>>>();
        }
        private readonly object _lockData = new object();
        public void AddOrUpdateStepData(int key, object value)
        {
            lock (_lockData)
            {
                var isNeedUpdateAndRaise = !_parameter.ContainsKey(key) || !_parameter[key].Equals(value);
                if (!isNeedUpdateAndRaise) return;
                _parameter[key] = value;
                RaiseEvent(key);
            }

        }

        private void RaiseEvent(int key)
        {
            lock (_lockObserve)
            {
                if (!_observe.ContainsKey(key)) return;
                foreach (var events in _observe[key])
                {
                    events(key);
                }
            }
        }

        private readonly object _lockObserve = new object();
        public void AddObserveByKey(int key, Action<int> observeAction)
        {
            lock (_lockObserve)
            {
                if (_observe.ContainsKey(key))
                {
                    _observe[key].Add(observeAction);
                    return;
                }
                _observe[key] = new List<Action<int>>() { observeAction };
            }
        }

        public T GetDataByKey<T>(int key)
        {
            lock (_lockData)
            {
                return (T) _parameter[key];
            }
        }
    }
}
