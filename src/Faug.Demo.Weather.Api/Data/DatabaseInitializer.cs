using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Faug.Demo.Weather.Api.Data.Context;

namespace Faug.Demo.Weather.Api.Data
{
    public static class DatabaseInitializer
    {
        public static async Task EnsureDatabaseAsync(DbContext dbContext, CancellationToken cancellationToken)
        {
            var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                // Create the database if it does not exist.
                // Do this first so there is then a database to start a transaction against.
                if (!await dbCreator.ExistsAsync(cancellationToken))
                {
                    await dbCreator.CreateAsync(cancellationToken);
                }
            });
        }
        public static async Task SeedDataAsync(WeatherContext dbContext, CancellationToken cancellationToken)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                foreach (var weatherForecast in SeedWeatherData.WeatherForecasts())
                {
                    await dbContext.WeatherForecasts.AddAsync(weatherForecast);
                }
                await dbContext.SaveChangesAsync(cancellationToken);
            });
        }

        public static async Task RunMigrationAsync(DbContext dbContext, CancellationToken cancellationToken)
        {
            var strategy = dbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
            });
        }
    }
}
