namespace TGProV4.Application.Interfaces.Repositories;

public interface IRepositoryBase<T, in TId> where T : class, IEntity<TId>
{
    IQueryable<T> Entities { get; }

    Task<bool> IsEntityExists(Expression<Func<T, bool>> predicate);

    IQueryable<T> GetEntities(Expression<Func<T, bool>>? predicate = null,
                              Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                              string includeProperties = "");

    IQueryable<T> GetPagedResponse(int pageNumber,
                                   int pageSize,
                                   Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                   string includeProperties = "");

    Task<T> AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}
