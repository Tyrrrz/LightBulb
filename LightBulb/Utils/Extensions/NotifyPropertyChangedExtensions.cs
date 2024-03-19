using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reflection;

namespace LightBulb.Utils.Extensions;

internal static class NotifyPropertyChangedExtensions
{
    public static IDisposable WatchProperty<TOwner, TProperty>(
        this TOwner owner,
        Expression<Func<TOwner, TProperty>> propertyExpression,
        Action handle
    )
        where TOwner : INotifyPropertyChanged
    {
        if (propertyExpression.Body is not MemberExpression { Member: PropertyInfo property })
            throw new ArgumentException("Provided expression must reference a property.");

        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (string.Equals(args.PropertyName, property.Name, StringComparison.Ordinal))
                handle();
        }

        owner.PropertyChanged += OnPropertyChanged;

        return Disposable.Create(() => owner.PropertyChanged -= OnPropertyChanged);
    }
}
