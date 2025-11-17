using System.ComponentModel.DataAnnotations;
using Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Api;

public class Program
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<AppOptions>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var appOptions = new AppOptions();
            configuration.GetSection(nameof(AppOptions)).Bind(appOptions);
            return appOptions;
        });
        //Add DB Context here
        //Example:
        /*services.AddDbContext<DbContext>((services, options) =>
        {
            options.UseNpgsql(services.GetRequiredService<AppOptions>().DbConnectionString);
        });*/
        services.AddControllers();
        services.AddOpenApiDocument();
        services.AddCors();
        //Add services here
        //Example:
        services.AddScoped<IServiceExample, ServiceExample>();
        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();
    }

    public static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        ConfigureServices(builder.Services);
        var app = builder.Build();


        var appOptions = app.Services.GetRequiredService<AppOptions>();
        Validator.ValidateObject(appOptions, new ValidationContext(appOptions), true);
        app.UseExceptionHandler(config => { });
        app.UseOpenApi();
        app.UseSwaggerUi();
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().SetIsOriginAllowed(x => true));
        app.MapControllers();
        app.GenerateApiClientsFromOpenApi("/../../client/src/generated-ts-client.ts");
        app.Run();
    }
}