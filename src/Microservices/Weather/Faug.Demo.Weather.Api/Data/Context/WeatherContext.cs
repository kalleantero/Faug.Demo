using Faug.Demo.Weather.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faug.Demo.Weather.Api.Data.Context;

public partial class WeatherContext : DbContext
{
    public WeatherContext(DbContextOptions<WeatherContext> options)
        : base(options)
    {
    }

    public virtual DbSet<WeatherForecast> WeatherForecasts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapDataModels(modelBuilder);
    }

    static void MapDataModels(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id);
            entity.Property(e => e.City);
            entity.Property(e => e.DateTime);
            entity.Property(e => e.TemperatureC);
            entity.Property(e => e.Summary);
        });
    }
}
