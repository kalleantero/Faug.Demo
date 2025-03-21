using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using Faug.Demo.ServiceDefaults;
using Faug.Demo.Frontend.BFF;
using Faug.Demo.Frontend.Endpoints;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.HttpOverrides;


var builder = WebApplication.CreateBuilder(args);

//************ASPIRE STUFF************

builder.AddServiceDefaults();

//************SPA CONFIG************

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/build";
});

//************REVERSE PROXY************

var backendConfig = builder.Configuration.GetSection("ReverseProxyRulesForBackend") ?? throw new ArgumentException();

builder.Services.AddReverseProxy()
    .LoadFromConfig(backendConfig)
    .AddTransforms<AuthorizationTransformProvider>() // AuthorizationTransformProvider gets Access token from cookie and appends it to the request
    .AddServiceDiscoveryDestinationResolver(); // Service Discovery


//************AUTHENTICATION************

var clientSecret = builder.Configuration.GetValue<string>("frontend-client-secret") ?? throw new ArgumentException();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "__BFF";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
})
.AddKeycloakOpenIdConnect(
    serviceName: "keycloak-idp",
    realm: "master",
    options =>
    {
        options.Authority = "https://keycloak-idp/realms/master";
        options.RequireHttpsMetadata = builder.Environment.IsDevelopment() ? true : true;
        options.ClientId = "frontend";
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.ClientSecret = clientSecret;
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("offline_access");
        options.SaveTokens = true;
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.TokenValidationParameters = new()
        {
            NameClaimType = "name",
            RoleClaimType = "role",
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });

//************AUTHORIZATION************

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUserPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

if (!builder.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();

// Enable authentication middleware in pipeline
app.UseAuthentication();

// Enable routing middleware in pipeline
app.UseAuthorization();

//************ENDPOINTS************

app.UseCors();

app.MapDefaultEndpoints();

#pragma warning disable ASP0014
app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy();
    endpoints.MapLoginAndLogout();
});
#pragma warning restore ASP0014


//************SPA CONFIG************

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (builder.Environment.IsDevelopment())
    {
        spa.UseReactDevelopmentServer(npmScript: "start");
    }
});

app.Run();

