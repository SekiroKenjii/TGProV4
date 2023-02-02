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

    public static void AddCloudService(this IServiceCollection services)
    {
        services.AddTransient(typeof(IImageService<>), typeof(ImageService<>));
        services.AddTransient<IMailService, MailService>();
    }

    public static void AddIdentityService(this IServiceCollection services)
    {
        services
           .AddTransient<ITokenService, IdentityService>()
           .AddTransient<IUserService, UserService>();
    }

    public static void AddServerStorage(this IServiceCollection services) { services.AddServerStorage(null); }

    private static void AddServerStorage(this IServiceCollection services,
                                         Action<SystemTextJsonOptions>? configure)
    {
        services
           .AddScoped<IJsonSerializer, SystemTextJsonSerializer>()
           .AddScoped<IStorageProvider, ServerStorageProvider>()
           .AddScoped<IAsyncServerStorageService, ServerStorageService>()
           .AddScoped<ISyncServerStorageService, ServerStorageService>()
           .Configure<SystemTextJsonOptions>(configureOptions => {
                configure?.Invoke(configureOptions);

                if (configureOptions.JsonSerializerOptions.Converters.All(c
                        => c.GetType() != typeof(TimespanJsonConverter)))
                {
                    configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                }
            });
    }
}
