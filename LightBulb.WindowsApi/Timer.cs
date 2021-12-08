using System;
using System.Threading;

namespace LightBulb.WindowsApi;

public partial class Timer : IDisposable
{
    private readonly object _lock = new();

    private readonly Action _callback;
    private readonly TimeSpan _firstTickInterval;
    private readonly TimeSpan _interval;

    private readonly System.Threading.Timer _internalTimer;

    private bool _isActive;
    private bool _isBusy;
    private bool _isDisposed;

    public Timer(TimeSpan firstTickInterval, TimeSpan interval, Action callback)
    {
        _firstTickInterval = firstTickInterval;
        _interval = interval;
        _callback = callback;

        _internalTimer = new System.Threading.Timer(
            _ => Tick(),
            null,
            Timeout.InfiniteTimeSpan,
            Timeout.InfiniteTimeSpan
        );
    }

    public Timer(TimeSpan interval, Action callback)
        : this(TimeSpan.Zero, interval, callback)
    {
    }

    private void Tick()
    {
        // Prevent multiple reentry
        if (_isBusy)
            return;

        lock (_lock)
        {
            // Prevent executing when already disposed (race condition)
            if (_isDisposed)
                return;

            try
            {
                _isBusy = true;
                _callback();
            }
            finally
            {
                _isBusy = false;
            }
        }
    }

    public void Start()
    {
        if (_isActive) return;

        _internalTimer.Change(_firstTickInterval, _interval);
        _isActive = true;
    }

    public void Stop()
    {
        if (!_isActive) return;

        _internalTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        _isActive = false;
    }

    public void Dispose()
    {
        // Lock so that tick doesn't trigger after dispose (bad idea?)
        lock (_lock)
        {
            _isActive = false;
            _isDisposed = true;
            _internalTimer.Dispose();
        }
    }
}

public partial class Timer
{
    public static IDisposable QueueDelayedAction(TimeSpan delay, Action callback)
    {
        var timer = new Timer(delay, Timeout.InfiniteTimeSpan, callback);
        timer.Start();

        return timer;
    }
}