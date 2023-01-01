using System;
using System.Collections.Generic;

namespace DailyProject_221204
{
    public class DisposeComposer : IDisposable
    {
        public DisposeComposer()
        {
        }
        public DisposeComposer(IEnumerable<IDisposable> disposables)
        {
            Add(disposables);
        }

        HashSet<IDisposable> _disposables = new();

        public void Add(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }
        public void Add(IEnumerable<IDisposable> disposables)
        {
            foreach (var item in disposables)
            {
                _disposables.Add(item);
            }
        }

        public void Dispose()
        {
            foreach (var i in _disposables)
            {
                i.Dispose();
            }
            _disposables.Clear();
        }
    }
}
