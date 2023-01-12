namespace TGProV4.Infrastructure.Repositories;

public class RepositoryBase<T, TId> : IRepositoryBase<T, TId> where T : AuditableEntity<TId>
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _db;

    public RepositoryBase(ApplicationDbContext context)
    {
        _context = context;
        _db = _context.Set<T>();
    }

    public IQueryable<T> Entities {
        get => _db;
    }

    public async Task<bool> IsEntityExists(Expression<Func<T, bool>> predicate)
    {
        return await _db.AnyAsync(predicate);
    }

    public IQueryable<T> GetEntities(Expression<Func<T, bool>>? predicate = null,
                                     Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                     string includeProperties = "")
    {
        var query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Aggregate<string?, IQueryable<T>>(_db, (current, includeProperty)
                                          => includeProperty != null
                                              ? current.Include(includeProperty)
                                              : current);

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query;
    }

    public IQueryable<T> GetPagedResponse(int pageNumber,
                                          int pageSize,
                                          Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                          string includeProperties = "")
    {
        var query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Aggregate<string?, IQueryable<T>>(_db, (current, includeProperty)
                                          => includeProperty != null
                                              ? current.Include(includeProperty)
                                              : current);

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        return query.Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _db.AddAsync(entity);
        return entity;
    }

    public Task UpdateAsync(T entity)
    {
        var entityEntry = _context.Entry(entity);

        if (entityEntry.State != EntityState.Detached)
        {
            _db.Attach(entity);
            _db.Update(entity);
        }

        entityEntry.State = EntityState.Modified;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        _db.Remove(entity);
        return Task.CompletedTask;
    }
}
