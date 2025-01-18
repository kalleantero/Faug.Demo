using Faug.Demo.ServiceDefaults;
using Faug.Demo.Weather.Api.Data;
using Faug.Demo.Weather.Api.Data.Context;
using Faug.Demo.Weather.Api.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

//************ASPIRE STUFF************

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//************AUTHENTICATION************

builder.Services.AddAuthentication()
                .AddKeycloakJwtBearer(
                    serviceName: "keycloak-idp",
                    realm: "master",
                    options =>
                    {
                        //options.Audience = "company.api";
                        options.Authority = "https://keycloak-idp/realms/master";

                        options.RequireHttpsMetadata = builder.Environment.IsDevelopment() ? true : true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateIssuerSigningKey = true,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero,
                            // ValidAudiences = allowedInternalAudiences.Concat(allowedUiAudiences)
                        };
                    });

//************AUTHORIZATION************

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy =
            new AuthorizationPolicyBuilder("Bearer")
            .RequireAuthenticatedUser()
            .Build();

});


// Register DbContext class
builder.AddSqlServerDbContext<WeatherContext>("weather-db");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Retrieve an instance of the DbContext class and manually run migrations during startup
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<WeatherContext>();

        CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken token = tokenSource.Token;

       // await DatabaseInitializer.EnsureDatabaseAsync(context, token);

       // 

        context.Database.EnsureCreated();
        await DatabaseInitializer.SeedDataAsync(context, token);

        //context.Database.Migrate();
    }
}

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//************ENDPOINTS************

app.MapEndpoints();

app.Run();

