using System;
using System.Collections.Generic;
using LightBulb.Utils.Extensions;

namespace LightBulb.Utils;

internal class DisposablePool : IDisposable
{
    private readonly List<IDisposable> _items = [];

    public void Add(IDisposable item) => _items.Add(item);

    public void Dispose()
    {
        _items.DisposeAll();
        _items.Clear();
    }
}
