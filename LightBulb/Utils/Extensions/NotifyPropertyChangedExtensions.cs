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
        Action callback,
        bool watchInitialValue = false
    )
        where TOwner : INotifyPropertyChanged
    {
        var memberExpression = propertyExpression.Body as MemberExpression;
        if (memberExpression?.Member is not PropertyInfo property)
            throw new ArgumentException("Provided expression must reference a property.");

        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (
                string.IsNullOrWhiteSpace(args.PropertyName)
                || string.Equals(args.PropertyName, property.Name, StringComparison.Ordinal)
            )
            {
                callback();
            }
        }

        owner.PropertyChanged += OnPropertyChanged;

        if (watchInitialValue)
            callback();

        return Disposable.Create(() => owner.PropertyChanged -= OnPropertyChanged);
    }

    public static IDisposable WatchProperties<TOwner>(
        this TOwner owner,
        IReadOnlyList<Expression<Func<TOwner, object?>>> propertyExpressions,
        Action callback,
        bool watchInitialValue = false
    )
        where TOwner : INotifyPropertyChanged
    {
        var properties = propertyExpressions
            .Select(expression =>
            {
                var memberExpression =
                    expression.Body as MemberExpression
                    // Because the expression is typed to return an object, the compiler will
                    // implicitly wrap it in a conversion unary expression if it's of any other type.
                    ?? (expression.Body as UnaryExpression)?.Operand as MemberExpression;

                if (memberExpression?.Member is not PropertyInfo property)
                    throw new ArgumentException("Provided expression must reference a property.");

                return property;
            })
            .ToArray();

        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (
                string.IsNullOrWhiteSpace(args.PropertyName)
                || properties.Any(p =>
                    string.Equals(args.PropertyName, p.Name, StringComparison.Ordinal)
                )
            )
            {
                callback();
            }
        }

        owner.PropertyChanged += OnPropertyChanged;

        if (watchInitialValue)
            callback();

        return Disposable.Create(() => owner.PropertyChanged -= OnPropertyChanged);
    }

    public static IDisposable WatchAllProperties<TOwner>(
        this TOwner owner,
        Action callback,
        bool watchInitialValues = false
    )
        where TOwner : INotifyPropertyChanged
    {
        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args) => callback();
        owner.PropertyChanged += OnPropertyChanged;

        if (watchInitialValues)
            callback();

        return Disposable.Create(() => owner.PropertyChanged -= OnPropertyChanged);
    }
}
