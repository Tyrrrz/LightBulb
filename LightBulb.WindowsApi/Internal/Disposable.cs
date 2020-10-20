using System;
using System.Collections.Generic;

namespace LightBulb.WindowsApi.Internal
{
    internal partial class Disposable : IDisposable
    {
        private readonly Action _dispose;

        public Disposable(Action dispose) =>
            _dispose = dispose;

        public void Dispose() => _dispose();
    }

    internal partial class Disposable
    {
        public static IDisposable Create(Action dispose) => new Disposable(dispose);

        public static IDisposable Aggregate(IReadOnlyList<IDisposable?> disposables) => Create(() =>
        {
            foreach (var i in disposables)
                i?.Dispose();
        });

        public static IDisposable Aggregate(params IDisposable?[] disposables) =>
            Aggregate((IReadOnlyList<IDisposable?>) disposables);
    }
}