namespace TGProV4.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddServerLocalization(this IServiceCollection services)
    {
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(ServerLocalizer<>));
    }

    public static AppConfiguration GetApplicationConfigurations(this IServiceCollection services,
                                                                IConfiguration configuration)
    {
        var appConfig = configuration.GetSection(nameof(AppConfiguration));
        services.Configure<AppConfiguration>(appConfig);
        return appConfig.Get<AppConfiguration>() ?? throw new InvalidOperationException();
    }

    public static void ConfigureCloudinaryService(this IServiceCollection services, IConfiguration configuration)
    {
        var cloudinaryConfig = configuration.GetSection(nameof(CloudinaryConfiguration));
        services.Configure<CloudinaryConfiguration>(cloudinaryConfig);
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

    public static void ConfigureRouteService(this IServiceCollection services)
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
        services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>();
        services
           .AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>()
           .Configure<SystemTextJsonOptions>(configureOptions => {
                if (configureOptions.JsonSerializerOptions.Converters.All(c
                        => c.GetType() != typeof(TimespanJsonConverter)))
                {
                    configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
                }
            });
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

        services.AddAuthorization(options => {
            foreach (var field in typeof(ApplicationPermissions)
                                 .GetNestedTypes()
                                 .SelectMany(c => c.GetFields(BindingFlags.Public |
                                                              BindingFlags.Static |
                                                              BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = field.GetValue(null)?.ToString();

                if (!string.IsNullOrEmpty(propertyValue))
                {
                    options.AddPolicy(propertyValue, policy
                        => policy.RequireClaim(ApplicationConstants.ClaimTypes.Permission, propertyValue));
                }
            }
        });
    }

    public static void RegisterHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
        services.AddHangfireServer();
    }

    public static void RegisterApiVersioning(this IServiceCollection services, AppConfiguration config)
    {
        services
           .AddApiVersioning(options => {
                options.DefaultApiVersion = new ApiVersion(config.ApiMajorVersion, config.ApiMinorVersion);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
           .AddApiExplorer(options => {
                options.GroupNameFormat =
                    config.ApiVersionGroupNameFormat ??
                    throw new Exception(StringHelpers.Message.MissedConfig("ApiVersionGroupNameFormat"));
                options.SubstituteApiVersionInUrl = true;
            });
    }
}
