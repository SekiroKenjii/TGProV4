namespace TGProV4.Infrastructure.Repositories;

public class RepositoryBase<T, TId> : IRepositoryBase<T, TId> where T : AuditableEntity<TId>
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _db;

    protected RepositoryBase(ApplicationDbContext context)
    {
        _context = context;
        _db = _context.Set<T>();
    }
    
    public IQueryable<T> Entities => _db;
    
    public async Task<T> AddAsync(T entity)
    {
        await _db.AddAsync(entity);
        return entity;
    }

    public Task DeleteAsync(T entity)
    {
        _db.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<List<T>> GetAllAsync()
    {
        IQueryable<T> query = _db;

        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(TId id)
    {
        return await _db.FindAsync(id);
    }

    public async Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
    {
        return await _db.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetFirstAsync(Expression<Func<T, bool>>? expression = null)
    {
        IQueryable<T> query = _db;

        if (expression is null)
        {
            return await query.FirstOrDefaultAsync();
        }

        return await query.FirstOrDefaultAsync(expression);
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
}