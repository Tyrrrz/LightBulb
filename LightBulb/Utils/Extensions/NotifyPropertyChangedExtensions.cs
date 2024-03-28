using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        Action handle,
        bool watchInitialValue = true
    )
        where TOwner : INotifyPropertyChanged
    {
        if (propertyExpression.Body is not MemberExpression { Member: PropertyInfo property })
            throw new ArgumentException("Provided expression must reference a property.");

        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (
                string.IsNullOrWhiteSpace(args.PropertyName)
                || string.Equals(args.PropertyName, property.Name, StringComparison.Ordinal)
            )
            {
                handle();
            }
        }

        owner.PropertyChanged += OnPropertyChanged;

        if (watchInitialValue)
            handle();

        return Disposable.Create(() => owner.PropertyChanged -= OnPropertyChanged);
    }

    public static IDisposable WatchAllProperties<TOwner>(
        this TOwner owner,
        Action handle,
        bool watchInitialValues = true
    )
        where TOwner : INotifyPropertyChanged
    {
        void OnPropertyChanged(object? sender, PropertyChangedEventArgs args) => handle();

        owner.PropertyChanged += OnPropertyChanged;

        if (watchInitialValues)
            handle();

        return Disposable.Create(() => owner.PropertyChanged -= OnPropertyChanged);
    }

    public static IDisposable WatchCollection<T>(
        this ObservableCollection<T> collection,
        Action handle,
        bool watchInitialValues = true
    )
    {
        void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs args) => handle();

        collection.CollectionChanged += OnCollectionChanged;

        if (watchInitialValues)
            handle();

        return Disposable.Create(() => collection.CollectionChanged -= OnCollectionChanged);
    }
}
