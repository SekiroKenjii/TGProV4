namespace TGProV4.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services
            .AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>))
            .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
    }
}
