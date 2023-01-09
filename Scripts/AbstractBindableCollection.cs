using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using DailyProject_221204.Scripts;

namespace DailyProject_221204
{
    /// <summary>
    /// ObservableCollectionのラッパークラス。
    /// itemとIDisposableのBindを追加。
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public abstract class AbstractBindableCollection<TItem> : IDisposable, ICollection<TItem>, INotifyCollectionChanged, INotifyPropertyChanged where TItem : class
    {
        protected AbstractBindableCollection()
        {
            _collection.CollectionChanged += (a, b) =>
            {
                if (CollectionChanged != null)
                {
                    CollectionChanged.Invoke(a, b);
                }
            };
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public event PropertyChangedEventHandler? PropertyChanged;

        readonly EventPublisher<TItem> _addEventPublisher = new();
        readonly EventPublisher<TItem> _removeEventPublisher = new();
        readonly EventPublisher _collectionChangeEventPublisher = new();
        readonly Dictionary<TItem, DisposeComposer> _modelToDisposables = new();
        readonly protected ObservableCollection<TItem> _collection = new();

        public int Count => _collection.Count;
        public bool IsReadOnly => false;

        public IDisposable SubscribeAdd(Action<TItem> action)
        {
            return _addEventPublisher.Subscribe(action);
        }
        public IDisposable SubscribeRemove(Action<TItem> action)
        {
            return _removeEventPublisher.Subscribe(action);
        }
        public IDisposable SubscribeCollectionChange(Action action)
        {
            return _collectionChangeEventPublisher.Subscribe(action);
        }

        public void Dispose()
        {
            if(CollectionChanged != null)
            {
                _collection.CollectionChanged -= CollectionChanged.Invoke;
            }
            foreach (var key in _modelToDisposables.Keys.ToArray())
            {
                _modelToDisposables[key].Dispose();
            }

            Clear();
        }
        public IEnumerator<TItem> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        protected virtual void __bind(TItem item)
        {
        }
        protected virtual void __unbind(TItem item)
        {
        }
        protected virtual IEnumerable<TItem> _sort(IEnumerable<TItem> items)
        {
            return items;
        }

        protected void _sort()
        {
            var sorted = _sort(_collection);

            for (int i = 0; i < sorted.Count(); i++)
            {
                _collection.Move(_collection.IndexOf(sorted.ElementAt(i)), i);
            }
        }

        protected void _subscribe(TItem item, IEnumerable<IDisposable> disposables)
        {
            if (_modelToDisposables.ContainsKey(item) == false)
            {
                var composer = new DisposeComposer();
                _modelToDisposables[item] = composer;
            }
            foreach (var disposable in disposables)
            {
                _modelToDisposables[item].Add(disposable);
            }
        }

        public void Sort()
        {
            _sort();
        }

        public void Add(TItem item)
        {
            if (_collection.Contains(item) == true)
            {
                throw new InvalidOperationException($"すでに登録されているModelが追加されました。[model]{item}");
            }

            _collection.Add(item);
            _sort();

            _bind(item);

            _addEventPublisher.Publish(item);
            _collectionChangeEventPublisher.Publish();
        }
        public void Add(IEnumerable<TItem> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
        public bool TryAdd(TItem item)
        {
            if (_collection.Contains(item) == true)
            {
                return false;
            }

            Add(item);

            return true;
        }

        public bool Remove(TItem item)
        {
            if (_collection.Contains(item) == false)
            {
                throw new InvalidOperationException();
            }

            var couldRemoved = _collection.Remove(item);
            _sort();

            _unbind(item);

            _removeEventPublisher.Publish(item);
            _collectionChangeEventPublisher.Publish();

            return couldRemoved;
        }
        public bool TryRemove(TItem item)
        {
            if (_collection.Contains(item) == false)
            {
                return false;
            }

            Remove(item);

            return true;
        }

        public bool Contains(TItem item)
        {
            return _collection.Contains(item);
        }
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public void Clear()
        {
            foreach (var item in _collection.ToArray())
            {
                Remove(item);
            }
        }

        void _bind(TItem item)
        {
            __bind(item);
        }
        void _unbind(TItem item)
        {
            __unbind(item);
            _unsubscribe(item);
        }

        void _unsubscribe(TItem item)
        {
            if (_modelToDisposables.TryGetValue(item, out var disposeComposer))
            {
                disposeComposer.Dispose();
            }
        }

        void _onPropertyChanged()
        {
            if(PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(this.GetType().Name));
            }
        }
    }
}
