var builder = WebApplication.CreateBuilder(args);

// Configuration Manager
var config = builder.Configuration;

// Service Collection
var services = builder.Services;

// Add services to the container.
services.AddSerialization();

services.AddApplicationLayer();

services.AddDatabase(config);

services.AddCurrentUserService();

services.ConfigureFluentValidation();

services.AddJwtAuthentication(services.GetApplicationSettings(config));

services.AddRepositories();

services.RegisterSwagger();

services.AddEndpointsApiExplorer();

services.ConfigureRoute();

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

app.Run();
