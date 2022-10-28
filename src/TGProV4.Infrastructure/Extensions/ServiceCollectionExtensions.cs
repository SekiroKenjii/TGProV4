using Microsoft.Extensions.DependencyInjection;
using TGProV4.Application.Interfaces.Repositories;
using TGProV4.Infrastructure.Repositories;

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