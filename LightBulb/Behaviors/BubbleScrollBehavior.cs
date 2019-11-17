﻿using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace LightBulb.Behaviors
{
    public class BubbleScrollBehavior : Behavior<UIElement>
    {
        private void AssociatedObject_OnPreviewMouseWheel(object? sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            var e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.MouseWheelEvent
            };

            AssociatedObject.RaiseEvent(e2);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += AssociatedObject_OnPreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_OnPreviewMouseWheel;
            base.OnDetaching();
        }
    }
}