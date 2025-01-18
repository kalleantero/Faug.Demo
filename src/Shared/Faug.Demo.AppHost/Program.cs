using Aspire.Hosting;
using Azure.Provisioning;
using Faug.Demo.AppHost.Util;
using k8s.Models;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);


/*
var keycloakRealmName = builder.AddParameter("keycloak-realm");
var keycloakRealmDisplayName = builder.AddParameter("keycloak-realm-display");
var frontendClientId = builder.AddParameter("frontend-client-id");
var frontendClientName = builder.AddParameter("frontend-client-name");
var frontendClientSecret = builder.AddParameter("frontend-client-secret", secret: true)
    .WithGeneratedDefault(new() { MinLength = 32, Special = false });
*/

var httpPort = 80;
var httpsPort = 443;

//************KEYCLOAK************

//KeyCloak needs more configuration to work in Azure (HTTPS-configuration)!


var username = builder.AddParameter("keycloak-username");
var password = builder.AddParameter("keycloak-password", secret: true);
var keycloak = builder.AddKeycloak("keycloak-idp",null, username, password)
                                .WithDataVolume()
                                .WithExternalHttpEndpoints();

if (builder.Environment.IsDevelopment() && !builder.ExecutionContext.IsPublishMode)
{
    keycloak.RunWithHttpsDevCertificate();
}
//************WEATHER API************

var sqlPassword = builder.AddParameter("sql-password", secret: true);
var sql = builder.AddSqlServer("weather-db-server", sqlPassword, 1433)
                 .AddDatabase("weather-db");

var weatherapi = builder.AddProject<Projects.Faug_Demo_Weather_Api>("weather-api")
           .WithReference(keycloak)           
           .WaitFor(keycloak)
           .WithReference(sql)
           .WaitFor(sql)
           .WithHttpEndpoint(port: (builder.ExecutionContext.IsPublishMode || !builder.Environment.IsDevelopment() ? 80 : 1080)).WithExternalHttpEndpoints() // Ingress;
           .WithHttpsEndpoint(port: (builder.ExecutionContext.IsPublishMode || !builder.Environment.IsDevelopment() ? 443 : 1443)).WithExternalHttpEndpoints();

//************LOCATION API************

var redis = builder.AddRedis("location-cache");

var location = builder.AddProject<Projects.Faug_Demo_Location_Api>("location-api")      
           .WithReference(keycloak)
           .WaitFor(keycloak)
           .WithReference(redis)
           .WaitFor(redis)
           .WithHttpEndpoint(port: (builder.ExecutionContext.IsPublishMode || !builder.Environment.IsDevelopment() ? 80 : 2080)).WithExternalHttpEndpoints()
           .WithHttpsEndpoint(port: (builder.ExecutionContext.IsPublishMode || !builder.Environment.IsDevelopment() ? 443 : 2443)).WithExternalHttpEndpoints();

//************FRONTEND************

var frontend = builder.AddProject<Projects.Faug_Demo_Frontend>("frontend")
       .WaitFor(keycloak)
       .WithReference(keycloak)
       .WaitFor(weatherapi)
       .WithReference(weatherapi)
       .WaitFor(location)
       .WithReference(location)
       .WithHttpEndpoint(port: (builder.ExecutionContext.IsPublishMode || !builder.Environment.IsDevelopment() ? 80 : 3080)).WithExternalHttpEndpoints()
       .WithHttpsEndpoint(port: (builder.ExecutionContext.IsPublishMode || !builder.Environment.IsDevelopment() ? 443 : 3443)).WithExternalHttpEndpoints();

//************CONSOLE APP************

/*
var consoleAppCommand = Path.GetFullPath("../Faug.Demo.Weather.Migration/bin/Debug/net8.0/Faug.Demo.Weather.Migration.exe");
var consoleAppWorkingDirectory = Path.GetFullPath("../Faug.Demo.Weather.Migration/bin/Debug/net8.0/");
var consoleApp = builder.AddExecutable("tenantService", consoleAppCommand, consoleAppWorkingDirectory)
    .WithEndpoint(8020, 8021, "https", "tsEndpoint", "ASPNETCORE_HTTPS_PORTS");
*/

//************NPM APP************

/*
builder.AddNpmApp("react", "../AspireJavaScript.React")
    .WithReference(weatherapi)
    .WaitFor(weatherapi)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();
*/

builder.Build().Run();