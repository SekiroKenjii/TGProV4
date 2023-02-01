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

    public IQueryable<T> Model {
        get => _db;
    }

    public async Task<bool> IsEntityExists(Expression<Func<T, bool>> predicate)
    {
        return await _db.AnyAsync(predicate);
    }

    public async Task<TResult?> GetEntity<TResult>(Expression<Func<T, bool>> predicate,
                                                   Expression<Func<T, TResult>> selector,
                                                   CancellationToken cancellationToken,
                                                   Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        IQueryable<T> query = _db;

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        return await query.Where(predicate)
                          .Select(selector)
                          .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T?> GetEntity(Expression<Func<T, bool>> predicate,
                                    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        IQueryable<T> query = _db;

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<TResult>> GetEntities<TResult>(Expression<Func<T, TResult>> selector,
                                                           Expression<Func<T, bool>>? predicate = null,
                                                           Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                                           string includeProperties = "")
    {
        var query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Aggregate<string?, IQueryable<T>>(_db, (current, includeProperty)
                                          => includeProperty != null
                                              ? current.Include(includeProperty)
                                              : current);

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        return await query.Select(selector).ToListAsync();
    }

    public async Task<IEnumerable<T>> GetEntities(Expression<Func<T, bool>>? predicate = null,
                                            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                            string includeProperties = "")
    {
        var query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Aggregate<string?, IQueryable<T>>(_db, (current, includeProperty)
                                          => includeProperty != null
                                              ? current.Include(includeProperty)
                                              : current);

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        return await query.ToListAsync();
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
