namespace TGProV4.Application.Interfaces.Repositories;

public interface IRepositoryBase<T, in TId> where T : class, IEntity<TId>
{
    IQueryable<T> Entities { get; }

    Task<T?> GetByIdAsync(TId id);

    Task<List<T>> GetAllAsync();

    Task<T?> GetFirstAsync(Expression<Func<T, bool>>? expression = null);

    Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

    Task<T> AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}