using System.Reflection;
using TGProV4.Infrastructure.Services.Identity;

namespace TGProV4.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructureMappers(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services
            .AddTransient(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>))
            .AddTransient<IBrandRepository, BrandRepository>()
            .AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
    }

    public static void AddIdentityService(this IServiceCollection services)
    {
        services
            .AddTransient<ITokenService, IdentityService>()
            .AddTransient<IUserService, UserService>();
    }
}
