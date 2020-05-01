using System;

namespace LightBulb.Internal
{
    internal class Dirty<T> : IDisposable
    {
        private readonly object _lock = new object();

        private readonly Func<T> _getActualValue;
        private readonly Action<T> _dispose;

        private T _lastValue = default!;
        private bool _isDirty = true;

        public T Value => GetValue();

        public Dirty(Func<T> getValue, Action<T> dispose)
        {
            _getActualValue = getValue;
            _dispose = dispose;
        }

        private T GetValue()
        {
            lock (_lock)
            {
                if (_isDirty)
                {
                    if (_lastValue != null)
                        _dispose(_lastValue);

                    _lastValue = _getActualValue();
                    _isDirty = false;
                }

                return _lastValue;
            }
        }

        public void Invalidate()
        {
            lock (_lock)
            {
                _isDirty = true;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_lastValue != null)
                    _dispose(_lastValue);
            }
        }
    }

    internal static class Dirty
    {
        public static Dirty<T> Create<T>(Func<T> getActualValue, Action<T> dispose) =>
            new Dirty<T>(getActualValue, dispose);
    }
}