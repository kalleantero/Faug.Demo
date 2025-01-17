using Azure.Provisioning;
using Faug.Demo.AppHost.Util;
using k8s.Models;

var builder = DistributedApplication.CreateBuilder(args);


/*
var keycloakRealmName = builder.AddParameter("keycloak-realm");
var keycloakRealmDisplayName = builder.AddParameter("keycloak-realm-display");
var frontendClientId = builder.AddParameter("frontend-client-id");
var frontendClientName = builder.AddParameter("frontend-client-name");
var frontendClientSecret = builder.AddParameter("frontend-client-secret", secret: true)
    .WithGeneratedDefault(new() { MinLength = 32, Special = false });
*/

//************KEYCLOAK************

var username = builder.AddParameter("keycloak-username");
var password = builder.AddParameter("keycloak-password", secret: true);
var keycloak = builder.AddKeycloak("keycloak", 8080, username, password)
                                .WithDataVolume()
                                .WithExternalHttpEndpoints()
                                .RunWithHttpsDevCertificate();

//************WEATHER API************

var weatherDbServerName = builder.AddParameter("weather-db-server-name");
var weatherDbName = builder.AddParameter("weather-db-name");
var weatherDbPassword = builder.AddParameter("weather-db-password", secret: true);
var sqlPassword = builder.AddParameter("sql-password", secret: true);

var sql = builder.AddSqlServer("sql", sqlPassword, 61928)
                 .AddDatabase("sqldata");

var weatherapi = builder.AddProject<Projects.Faug_Demo_Weather_Api>("weatherapi")
           .WithReference(keycloak)
           .WithExternalHttpEndpoints()
           .WaitFor(keycloak)
           .WithReference(sql)
           .WaitFor(sql);

//************LOCATION API************

var redis = builder.AddRedis("location-cache");

var location = builder.AddProject<Projects.Faug_Demo_Location>("location")
           .WithExternalHttpEndpoints()
           .WithReference(keycloak)
           .WaitFor(keycloak)
           .WithReference(redis)
           .WaitFor(redis);

//************FRONTEND************

var frontend = builder.AddProject<Projects.Faug_Demo_Frontend>("frontendnew")
       .WithExternalHttpEndpoints()
       .WaitFor(keycloak)
       .WithReference(keycloak)
       .WaitFor(weatherapi)
       .WithReference(weatherapi)
       .WaitFor(location)
       .WithReference(location);

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