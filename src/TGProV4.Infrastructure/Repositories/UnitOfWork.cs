namespace TGProV4.Infrastructure.Repositories;

public class UnitOfWork<TId> : IUnitOfWork<TId>
{
    private readonly ApplicationDbContext _context;
    private bool _disposed;
    private Hashtable? _repositories;
    private readonly IAppCache _cache;

    public UnitOfWork(ApplicationDbContext context, IAppCache cache)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _cache = cache;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }

        _disposed = true;
    }

    public IRepositoryBase<T, TId> Repository<T>() where T : AuditableEntity<TId>
    {
        _repositories ??= new Hashtable();

        var type = typeof(T).Name;

        if (_repositories.ContainsKey(type))
        {
            return (IRepositoryBase<T, TId>) _repositories[type]!;
        }

        var repositoryType = typeof(RepositoryBase<,>);
        var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T), typeof(TId)),
            _context);

        _repositories.Add(type, repositoryInstance);

        return (IRepositoryBase<T, TId>) _repositories[type]!;
    }

    public async Task<int> Commit(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys)
    {
        var result = await _context.SaveChangesAsync(cancellationToken);

        foreach (var cacheKey in cacheKeys)
        {
            _cache.Remove(cacheKey);
        }

        return result;
    }

    public Task Rollback()
    {
        _context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());

        return Task.CompletedTask;
    }
}
