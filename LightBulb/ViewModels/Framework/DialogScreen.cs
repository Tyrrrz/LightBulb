﻿using System;

namespace LightBulb.ViewModels.Framework;

public abstract class DialogScreen<T> : PropertyChangedBase
{
    public T? DialogResult { get; private set; }

    public event EventHandler? Closed;

    public void Close(T dialogResult)
    {
        DialogResult = dialogResult;
        Closed?.Invoke(this, EventArgs.Empty);
    }
}

public abstract class DialogScreen : DialogScreen<bool?>;
