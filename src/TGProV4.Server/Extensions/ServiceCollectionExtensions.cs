using TGProV4.Infrastructure.Services;

namespace TGProV4.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static AppConfiguration GetApplicationSettings(this IServiceCollection services,
                                                          IConfiguration configuration)
    {
        var appSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        services.Configure<AppConfiguration>(appSettingsConfiguration);
        return appSettingsConfiguration.Get<AppConfiguration>();
    }

    public static void RegisterSwagger(this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options => options.OperationFilter<SwaggerDefaultValues>());
    }

    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services
           .AddDbContext<ApplicationDbContext>(options
                => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
           .AddTransient<IDataSeeder, DataSeeder>();
    }

    public static void AddIdentityUser(this IServiceCollection services)
    {
        services
           .AddIdentity<AppUser, AppRole>(options => {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
            })
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();
    }

    public static void AddCurrentUserService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
    }

    public static void ConfigureRoute(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }

    public static void ConfigureFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation(options => options.DisableDataAnnotationsValidation = true);
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<AppConfiguration>();
    }

    public static void AddSerialization(this IServiceCollection services)
    {
        services
           .AddControllers()
           .AddNewtonsoftJson(
                options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
    }

    public static void AddJwtAuthentication(this IServiceCollection services, AppConfiguration config)
    {
        var secret = config.Secret;

        if (string.IsNullOrEmpty(secret))
        {
            throw new Exception(StringHelpers.Message.MissedConfig("JWT"));
        }

        var key = Encoding.UTF8.GetBytes(secret);

        services
           .AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options => {
                var response = new Response<string> { Succeeded = false, Data = default };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents {
                    OnAuthenticationFailed = context => {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            response.Message = ApplicationConstants.Messages.TokenExpired;
                            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                        }
#if DEBUG
                        context.NoResult();
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "text/plain";
                        return context.Response.WriteAsync(context.Exception.ToString());
#else
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        response.Message = ApplicationConstants.Messages.InternalServerError;
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
#endif
                    },
                    OnChallenge = context => {
                        context.HandleResponse();

                        if (context.Response.HasStarted)
                        {
                            return Task.CompletedTask;
                        }

                        context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";
                        response.Message = ApplicationConstants.Messages.Unauthorized;
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                    },
                    OnForbidden = context => {
                        context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        response.Message = ApplicationConstants.Messages.Forbidden;
                        return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                    }
                };
            });
    }

    public static void ConfigApiVersioning(this IServiceCollection services)
    {
        services
           .AddApiVersioning(options => {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
           .AddApiExplorer(options => {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
    }
}
