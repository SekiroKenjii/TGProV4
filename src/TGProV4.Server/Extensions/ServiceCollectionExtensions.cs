using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TGProV4.Server.Extensions;

public static class ServiceCollectionExtensions
{
    public static AppConfiguration GetApplicationSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var appSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        services.Configure<AppConfiguration>(appSettingsConfiguration);
        return appSettingsConfiguration.Get<AppConfiguration>();
    }

    public static void RegisterSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TGProV4.CleanArchitecture",
                    License = new OpenApiLicense
                    {
                        Name = "MIT License", Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"<p>JWT Authorization header using the Bearer scheme.</br>
                                Enter 'Bearer' [space] and then your token in the text input below.</br>
                                Example: 'Bearer json-web-token'</p>",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        Scheme = "Bearer",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });
    }

    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDbContext<ApplicationDbContext>(options => options
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
    }

    public static void AddCurrentUserService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
    }

    public static void ConfigureRoute(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
        });
    }

    public static void ConfigureFluentValidation(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation(options =>
        {
            options.DisableDataAnnotationsValidation = true;
        });

        services.AddFluentValidationClientsideAdapters();

        services.AddValidatorsFromAssemblyContaining<AppConfiguration>();
    }

    public static void AddSerialization(this IServiceCollection services)
    {
        services.AddControllers()
            .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            );
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
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var response = new Response<string> { Succeeded = false, Data = default };

                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            response.Message = ApplicationConstants.Messages.TokenExpired;
                            var result = JsonSerializer.Serialize(response, jsonSerializerOptions);
                            return context.Response.WriteAsync(result);
                        }
                        else
                        {
#if DEBUG
                            context.NoResult();
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "text/plain";
                            return context.Response.WriteAsync(context.Exception.ToString());
#else
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "application/json";
                            response.Message = ApplicationConstants.Messages.InternalServerError;
                            var result = JsonSerializer.Serialize(response, jsonSerializerOptions);
                            return context.Response.WriteAsync(result);
#endif
                        }
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        if (context.Response.HasStarted)
                        {
                            return Task.CompletedTask;
                        }

                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        context.Response.ContentType = "application/json";
                        response.Message = ApplicationConstants.Messages.Unauthorized;
                        var result = JsonSerializer.Serialize(response, jsonSerializerOptions);
                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        response.Message = ApplicationConstants.Messages.Forbidden;
                        var result = JsonSerializer.Serialize(response, jsonSerializerOptions);
                        return context.Response.WriteAsync(result);
                    }
                };
            });
    }
}
