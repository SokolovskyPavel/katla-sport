using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KatlaSport.DataAccess;

namespace KatlaSport.Services.Tests
{
    public class FakeAsyncEntitySet<TEntity> : IEntitySet<TEntity>, IDbAsyncEnumerable<TEntity>
        where TEntity : class
    {
        private readonly IList<TEntity> _data;
        private readonly IQueryable _queryable;

        public FakeAsyncEntitySet(IList<TEntity> data)
        {
            _data = data;
            _queryable = _data.AsQueryable();
        }

        public ObservableCollection<TEntity> Local
        {
            get { throw new NotImplementedException(); }
        }

        Type IQueryable.ElementType
        {
            get { return _queryable.ElementType; }
        }

        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get { return _queryable.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new AsyncQueryProviderWrapper<TEntity>(_queryable.Provider); }
        }

        public int Count
        {
            get { return _data.Count; }
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            throw new NotImplementedException("Derive from FakeDbSet<T> and override Find");
        }

        public Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public TEntity Add(TEntity item)
        {
            _data.Add(item);
            return item;
        }

        public TEntity Remove(TEntity item)
        {
            _data.Remove(item);
            return item;
        }

        public TEntity Attach(TEntity item)
        {
            _data.Add(item);
            return item;
        }

        public TEntity Detach(TEntity item)
        {
            _data.Remove(item);
            return item;
        }

        public TEntity Create()
        {
            return Activator.CreateInstance<TEntity>();
        }

        public TDerivedEntity Create<TDerivedEntity>()
            where TDerivedEntity : class, TEntity
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        public IDbAsyncEnumerator<TEntity> GetAsyncEnumerator()
        {
            return new AsyncEnumeratorWrapper<TEntity>(_data.GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }
    }
}