using System;

namespace LightBulb.WindowsApi.Timers
{
    public interface ITimer : IDisposable
    {
        void Start();

        void Stop();
    }
}