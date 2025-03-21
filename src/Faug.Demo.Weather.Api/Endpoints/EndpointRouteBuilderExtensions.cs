using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Faug.Demo.Weather.Api.Data.Entities;
using Faug.Demo.Weather.Api.Data.Context;

namespace Faug.Demo.Weather.Api.Endpoints
{
    internal static class EndpointRouteBuilderExtensions
    {
        internal static IEndpointConventionBuilder MapEndpoints(
            this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/weather");

            group.MapGet("/ping", () =>
            {
                return Results.Ok("pong");
            }).AllowAnonymous();

            group.MapGet("/locations/{city}", async (string city, WeatherContext db) =>
            {
                var forecast = db.WeatherForecasts.Where(x => x.City == city).ToList();
                return Results.Ok(forecast);
            });

            return group;
        }
    }
}
