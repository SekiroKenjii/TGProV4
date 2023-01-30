namespace TGProV4.Server.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseExceptionHandling(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
    }

    public static void ConfigureSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", typeof(Program).Assembly.GetName().Name);
            options.RoutePrefix = "swagger";
            options.DisplayRequestDuration();
        });
    }

    public static void Initialize(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();

        var initializers = serviceScope.ServiceProvider.GetServices<IDataSeeder>();

        foreach (var initializer in initializers)
        {
            initializer.Run();
        }
    }
}
