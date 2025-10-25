using System.Collections.Generic;
using System.Linq;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using HouseHelp.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using HouseHelp.Api;

namespace HouseHelp.ApiTests.Fixtures;

public class ApiDatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlTestcontainer _postgres = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "househelp",
            Username = "app",
            Password = "app"
        })
        .WithCleanUp(true)
        .Build();

    private readonly RedisTestcontainer _redis = new TestcontainersBuilder<RedisTestcontainer>()
        .WithDatabase(new RedisTestcontainerConfiguration())
        .Build();

    public required WebApplicationFactory<Program> Factory { get; set; }

    public string ConnectionString => _postgres.ConnectionString;
    public string RedisConnection => _redis.ConnectionString;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await _redis.StartAsync();

        Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                var dict = new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Postgres"] = ConnectionString,
                    ["ConnectionStrings:Redis"] = RedisConnection
                };
                configBuilder.AddInMemoryCollection(dict!);
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseNpgsql(ConnectionString);
                });
            });
        });

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        var seeder = scope.ServiceProvider.GetRequiredService<AppDbSeeder>();
        await seeder.SeedAsync(CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
        await _redis.DisposeAsync();
        Factory.Dispose();
    }
}
