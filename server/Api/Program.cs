using System.ComponentModel.DataAnnotations;
using System.Text;
using Api.Services;
using DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Sieve.Services;

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
        
        services.AddDbContext<JerneDbContext>((services, options) =>
        {
            options.UseNpgsql(services.GetRequiredService<AppOptions>().DbConnectionString);
        });
        
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IServiceProvider>((options, sp) =>
            {
                var appOptions = sp.GetRequiredService<AppOptions>();
                var key = Encoding.UTF8.GetBytes(appOptions.Token);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = appOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = appOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true
                };
            });

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();
        
        services.AddControllers();
        services.AddOpenApi();
        services.AddOpenApiDocument();
        services.AddCors();

        services.AddScoped<ISieveProcessor, SieveProcessor>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IGameWinningNumberService, GameWinningNumberService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<BalanceHelper>();
        services.AddScoped<GameQueryHelper>();
        services.AddScoped<ISieveConfiguration, SieveConfiguration>();
        
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
        app.UseExceptionHandler();
        app.UseOpenApi();
        app.UseOpenApi();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi(); // serves /openapi
            app.MapScalarApiReference();
        }


        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().SetIsOriginAllowed(x => true));
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.GenerateApiClientsFromOpenApi("/../../client/src/generated-ts-client.ts");
        app.Run();
    }
}