namespace TGProV4.Infrastructure.Repositories;

public class UnitOfWork<TId> : IUnitOfWork<TId>
{
    private readonly ApplicationDbContext _context;
    private bool _disposed;
    private Hashtable? _repositories;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                //dispose managed resources
                _context.Dispose();
            }
        }
        //dispose unmanaged resources
        _disposed = true;
    }

    public IRepositoryBase<T, TId> Repository<T>() where T : AuditableEntity<TId>
    {
        _repositories ??= new Hashtable();

        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type)) return (IRepositoryBase<T, TId>)_repositories[type]!;
        
        var repositoryType = typeof(IRepositoryBase<,>);

        var repositoryInstance = Activator.CreateInstance
        (
            repositoryType.MakeGenericType(typeof(T), typeof(TId)),
            _context
        );

        _repositories.Add(type, repositoryInstance);

        return (IRepositoryBase<T, TId>)_repositories[type]!;
    }

    public async Task<int> Commit(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys)
    {
        throw new NotImplementedException();
    }

    public Task Rollback()
    {
        _context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        return Task.CompletedTask;
    }
}