using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Faug.Demo.Location.Models;
using System.Text.Json;
using System.IO;
using System.Reflection;

namespace Faug.Demo.Location.Endpoints
{
    internal static class EndpointRouteBuilderExtensions
    {
        internal static IEndpointConventionBuilder MapEndpoints(
            this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/locations/{country}");

            group.MapGet("/", GetLocationsByCountry)
                .WithName("GetLocationsByCountry")
                .AllowAnonymous()
                .WithOpenApi();

            return group;
        }

        static Faug.Demo.Location.Models.Location[] GetLocationsByCountry(string country)
        {
            string fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\fi.json");
            string jsonString = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<Faug.Demo.Location.Models.Location[]>(jsonString)!;
        }

    }
}

