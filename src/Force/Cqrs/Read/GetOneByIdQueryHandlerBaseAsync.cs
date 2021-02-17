using System;
using System.Linq;
using System.Threading.Tasks;
using Force.Cqrs.Delegates;
using Force.Ddd;

namespace Force.Cqrs.Read
{
    /// <summary>
    /// Base handler for async get entity by Id
    /// </summary>
    /// <typeparam name="TKey">Id type</typeparam>
    /// <typeparam name="TQuery">Input query</typeparam>
    /// <typeparam name="TEntity">Domain entity</typeparam>
    /// <typeparam name="TDto">Output dto</typeparam>
    public abstract class GetOneByIdQueryHandlerBaseAsync<TKey, TQuery, TEntity, TDto> :
        GetOneByIdMapBase<TQuery, TEntity, TDto>,
        IQueryHandler<TQuery, Task<TDto>>
        where TQuery : IQuery<Task<TDto>>, IHasId<TKey>
        where TDto : class, IHasId<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IQueryable<TEntity> _queryable;
        private readonly IServiceProvider _serviceProvider;

        protected GetOneByIdQueryHandlerBaseAsync(IQueryable<TEntity> queryable, IServiceProvider serviceProvider)
        {
            _queryable = queryable;
            _serviceProvider = serviceProvider;
        }

        public async Task<TDto> Handle(TQuery input)
        {
            var firstOrDefaultAsyncFunc =
                (FirstOrDefaultAsyncDelegate<TDto>) _serviceProvider.GetService(
                    typeof(FirstOrDefaultAsyncDelegate<TDto>)) ??
                throw new InvalidOperationException(typeof(FirstOrDefaultAsyncDelegate<TDto>).ToString());
            return await firstOrDefaultAsyncFunc(Map(_queryable, input), (x => x.Id.Equals(input.Id)));
        }
    }
}