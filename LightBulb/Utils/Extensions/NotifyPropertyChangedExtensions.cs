using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LightBulb.Utils.Extensions;

internal static class NotifyPropertyChangedExtensions
{
    public static IDisposable WatchProperty<TOwner, TProperty>(
        this TOwner owner,
        Expression<Func<TOwner, TProperty>> propertyExpression,
        Action handleChange,
        bool watchInitialValue = true
    )
        where TOwner : INotifyPropertyChanged
    {
        var memberExpression =
            propertyExpression.Body as MemberExpression
            // Property value might be boxed inside a conversion expression, if the types don't match
            ?? (propertyExpression.Body as UnaryExpression)?.Operand as MemberExpression;

        if (memberExpression?.Member is not PropertyInfo property)
            throw new ArgumentException("Provided expression must reference a property.");

        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (
                string.IsNullOrWhiteSpace(args.PropertyName)
                || string.Equals(args.PropertyName, property.Name, StringComparison.Ordinal)
            )
            {
                handleChange();
            }
        }

        owner.PropertyChanged += OnPropertyChanged;

        if (watchInitialValue)
            handleChange();

        return Disposable.Create(() => owner.PropertyChanged -= OnPropertyChanged);
    }

    public static IDisposable WatchProperties<TOwner>(
        this TOwner owner,
        IReadOnlyList<Expression<Func<TOwner, object?>>> propertyExpressions,
        Action handleChanges,
        bool watchInitialValue = true
    )
        where TOwner : INotifyPropertyChanged
    {
        var watchers = propertyExpressions
            .Select(x => WatchProperty(owner, x, handleChanges, watchInitialValue))
            .ToArray();

        return Disposable.Create(() => watchers.DisposeAll());
    }

    public static IDisposable WatchAllProperties<TOwner>(
        this TOwner owner,
        Action handleChanges,
        bool watchInitialValues = true
    )
        where TOwner : INotifyPropertyChanged
    {
        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args) => handleChanges();
        owner.PropertyChanged += OnPropertyChanged;

        if (watchInitialValues)
            handleChanges();

        return Disposable.Create(() => owner.PropertyChanged -= OnPropertyChanged);
    }
}
