using System;
using System.Collections.Generic;
using LightBulb.Utils.Extensions;

namespace LightBulb.Utils;

internal partial class Disposable : IDisposable
{
    private readonly Action _dispose;

    public Disposable(Action dispose) =>
        _dispose = dispose;

    public void Dispose() => _dispose();
}

internal partial class Disposable
{
    public static IDisposable Null { get; } = Create(() => { });

    public static IDisposable Create(Action dispose) => new Disposable(dispose);

    public static IDisposable Aggregate(IReadOnlyList<IDisposable> disposables) =>
        Create(disposables.DisposeAll);

    public static IDisposable Aggregate(params IDisposable[] disposables) =>
        Aggregate((IReadOnlyList<IDisposable>) disposables);
}