using System;
using System.ComponentModel;

namespace LightBulb.Internal
{
    internal static class Extensions
    {
        public static DateTimeOffset ResetTimeOfDay(this DateTimeOffset dateTime) =>
            new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTime.Offset);

        public static DateTimeOffset AtTimeOfDay(this DateTimeOffset dateTime, TimeSpan timeOfDay) =>
            dateTime.ResetTimeOfDay() + timeOfDay;

        public static DateTimeOffset NextTimeOfDay(this DateTimeOffset dateTime, TimeSpan timeOfDay) =>
            dateTime.TimeOfDay <= timeOfDay
                ? dateTime.AtTimeOfDay(timeOfDay)
                : dateTime.AddDays(1).AtTimeOfDay(timeOfDay);

        public static DateTimeOffset PreviousTimeOfDay(this DateTimeOffset dateTime, TimeSpan timeOfDay) =>
            dateTime.TimeOfDay > timeOfDay
                ? dateTime.AtTimeOfDay(timeOfDay)
                : dateTime.AddDays(-1).AtTimeOfDay(timeOfDay);

        public static void OpenInBrowser(this Uri uri) => ProcessEx.StartShellExecute(uri.ToString());

        public static void Bind<TSource>(this TSource target, EventHandler<PropertyChangedEventArgs> handler)
            where TSource : class, INotifyPropertyChanged
        {
            target.PropertyChanged += (sender, args) => handler(target, args);
        }

        public static void BindAndInvoke<TSource>(this TSource target, EventHandler<PropertyChangedEventArgs> handler)
            where TSource : class, INotifyPropertyChanged
        {
            target.Bind(handler);
            handler.Invoke(target, new PropertyChangedEventArgs(null));
        }
    }
}