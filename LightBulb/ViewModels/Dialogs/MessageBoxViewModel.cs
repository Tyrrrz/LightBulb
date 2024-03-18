﻿using LightBulb.ViewModels.Framework;

namespace LightBulb.ViewModels.Dialogs;

public class MessageBoxViewModel : DialogScreen
{
    public string? Title { get; set; }

    public string? Message { get; set; }

    public bool IsOkButtonVisible { get; set; } = true;

    public string? OkButtonText { get; set; }

    public bool IsCancelButtonVisible { get; set; }

    public string? CancelButtonText { get; set; }

    public int ButtonsCount => (IsOkButtonVisible ? 1 : 0) + (IsCancelButtonVisible ? 1 : 0);
}
