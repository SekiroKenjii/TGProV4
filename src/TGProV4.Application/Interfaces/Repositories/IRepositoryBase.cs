namespace TGProV4.Application.Interfaces.Repositories;

public interface IRepositoryBase<T, in TId> where T : class, IEntity<TId>
{
    IQueryable<T> Model { get; }
    Task<bool> IsEntityExists(Expression<Func<T, bool>> predicate);
    Task<TResult?> GetEntity<TResult>(Expression<Func<T, bool>> predicate,
                                      Expression<Func<T, TResult>> selector,
                                      CancellationToken cancellationToken,
                                      Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    Task<T?> GetEntity(Expression<Func<T, bool>> predicate,
                       Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    Task<IEnumerable<TResult>> GetEntities<TResult>(Expression<Func<T, TResult>> selector,
                                             Expression<Func<T, bool>>? predicate = null,
                                             Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                             string includeProperties = "");
    Task<IEnumerable<T>> GetEntities(Expression<Func<T, bool>>? predicate = null,
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
