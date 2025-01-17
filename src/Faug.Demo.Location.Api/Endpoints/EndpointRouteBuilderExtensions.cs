using System.Text.Json;
using System.Reflection;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Faug.Demo.Location.Api.Endpoints
{
    internal static class EndpointRouteBuilderExtensions
    {
        internal static IEndpointConventionBuilder MapEndpoints(
            this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/locations");

            group.MapGet("/{country}", async (string country, IDistributedCache cache) =>
            {
                var cachedLocations = await cache.GetAsync("locations");

                if (cachedLocations is null)
                {
                    var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\" + country + ".json");
                    var jsonString = File.ReadAllText(fileName);
                    var locations = JsonSerializer.Deserialize<Models.Location[]>(jsonString)!;

                    await cache.SetAsync("locations", Encoding.UTF8.GetBytes(JsonSerializer.Serialize(locations)), new()
                    {
                        AbsoluteExpiration = DateTime.Now.AddSeconds(10)
                    }); ;

                    return Results.Ok(locations);
                }

                return Results.Ok(JsonSerializer.Deserialize<Models.Location[]>(cachedLocations));
            })
            .WithName("GetLocationsByCountry")
            .AllowAnonymous()
            .WithOpenApi();

            return group;
        }

    }
}

