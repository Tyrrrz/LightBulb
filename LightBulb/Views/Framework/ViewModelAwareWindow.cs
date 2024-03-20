﻿using System;
using Avalonia.Controls;

namespace LightBulb.Views.Framework;

public class ViewModelAwareWindow<TDataContext> : Window
{
    public new TDataContext DataContext
    {
        get =>
            (TDataContext)(
                base.DataContext ?? throw new InvalidOperationException("DataContext is null.")
            );
        set => base.DataContext = value;
    }
}