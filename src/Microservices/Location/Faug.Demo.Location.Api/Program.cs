using Faug.Demo.Location.Api.Endpoints;
using Faug.Demo.ServiceDefaults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//************CACHE************

builder.AddRedisDistributedCache("location-cache");

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

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//************ENDPOINTS************

app.MapEndpoints();

app.Run();


