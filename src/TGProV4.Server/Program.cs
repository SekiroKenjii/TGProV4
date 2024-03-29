var builder = WebApplication.CreateBuilder(args);

// Configuration Manager
var config = builder.Configuration;

// Service Collection
var services = builder.Services;

// App Configuration
var appConfig = services.GetApplicationConfigurations(config);

// Add services to the container.
services.AddLocalization(options => {
    options.ResourcesPath = "Resources";
});
services.AddServerLocalization();
services.AddCurrentUserService();
services.AddControllers();
services.AddSerialization();
services.AddDatabase(config);
services.AddServerStorage();
services.AddScoped<ServerPreferenceManager>();
services.ConfigureCloudinaryService(config);
services.AddApplicationLayer();
services.AddIdentityUser();
services.AddCloudService();
services.AddIdentityService();
services.ConfigureFluentValidation();
services.AddJwtAuthentication(appConfig);
services.AddInfrastructureMappers();
services.AddRepositories();
services.RegisterSwagger();
services.RegisterHangfire(config);
services.RegisterApiVersioning(appConfig);
services.AddLazyCache();
services.ConfigureRouteService();
services.AddEndpointsApiExplorer();

// Web Application
var app = builder.Build();

// Environment
var env = app.Environment;

// Configure the HTTP request pipeline.
app.UseExceptionHandling(env);
app.ConfigureSwagger();
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Initialize();
app.ConfigureHangfire(); // Ensure that the database has been initialized
app.Run();
