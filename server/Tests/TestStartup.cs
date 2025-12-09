using Api;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;

namespace Tests;

public class Startup
{
    private static PostgreSqlContainer _dbContainer;
    
    public static void ConfigureServices(IServiceCollection services)
    {
        // Create fake in-memory configuration (like appsettings.json)
        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["AppOptions:Token"] = "super-secret-test-key-super-secret-test-key",
            ["AppOptions:Issuer"] = "TestIssuer",
            ["AppOptions:Audience"] = "TestAudience",
            ["AppOptions:DbConnectionString"] = "Server=localhost;Port=5432;Database=testdb;User Id=postgres;Password=postgres;"
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemoryConfig)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        
        Program.ConfigureServices(services);

        // Remove normal DB and use Testcontainer
        services.RemoveAll(typeof(JerneDbContext));

        _dbContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        _dbContainer.StartAsync().GetAwaiter().GetResult();

        services.AddScoped<JerneDbContext>(_ =>
        {
            var options = new DbContextOptionsBuilder<JerneDbContext>()
                .UseNpgsql(_dbContainer.GetConnectionString())
                .Options;

            var ctx = new JerneDbContext(options);
            ctx.Database.EnsureCreated();
            return ctx;
        });
    }
    }