﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LightBulb.Framework;

public abstract class ViewModelBase : ObservableObject, IDisposable
{
    ~ViewModelBase() => Dispose(false);

    protected void OnAllPropertiesChanged() => OnPropertyChanged(string.Empty);

    protected virtual void Dispose(bool disposing) { }

    public void Dispose() => Dispose(true);
}
