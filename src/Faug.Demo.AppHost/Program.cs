using Azure.Core;
using Azure.Provisioning;
using Faug.Demo.AppHost.Extensions;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var httpsPort = 443;
var environment = "Development";

#region Shared

//************SHARED************

/*
var keyVault = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureKeyVault("secrets")
    : builder.AddConnectionString("secrets");
*/

#endregion

#region Keycloak IDP

//************KEYCLOAK IDP************

var username = builder.AddParameter("keycloak-username");
var password = builder.AddParameter("keycloak-password", secret: true);

var keycloak = builder.AddKeycloak("keycloak-idp", null, username, password)
    .WithExternalHttpEndpoints();

if (builder.Environment.IsDevelopment() && !builder.ExecutionContext.IsPublishMode)
{
    //KeyCloak needs more configuration to work in Azure (certificate configuration)!
    keycloak.RunWithHttpsDevCertificate();
    keycloak.WithDataVolume();// data is persisted between restarts
}

#endregion

#region Weather

//************WEATHER API************

var weatherDbServerName = builder.AddParameter("weather-db-server-name");
var weatherDbName = builder.AddParameter("weather-db-name");
var weatherSqlPassword = builder.AddParameter("weather-db-admin-password", secret: true);

IResourceBuilder<IResourceWithConnectionString> weatherDatabase = null;

if (builder.Environment.IsDevelopment() && !builder.ExecutionContext.IsPublishMode)
{
    //local
    var sql = builder.AddLocalSqlServer(weatherDbServerName, weatherSqlPassword);
    weatherDatabase = sql.AddDatabase(weatherDbName.Resource.Value);
}
else
{
    //cloud
    var sql = builder.AddCloudSqlServer(weatherDbServerName, weatherSqlPassword);
    weatherDatabase = sql.AddDatabase(weatherDbName.Resource.Value);
}

var weatherapi = builder.AddProject<Projects.Faug_Demo_Weather_Api>("weather-api")
           .WithEnvironment("ASPNETCORE_ENVIRONMENT", environment)
           .WithReference(keycloak)           
           .WaitFor(keycloak)           
           .WithReference(weatherDatabase)
           .WaitFor(weatherDatabase)           
           .WithHttpsEndpoint(port: (builder.ExecutionContext.IsPublishMode || !builder.Environment.IsDevelopment() ? httpsPort : 1443)).WithExternalHttpEndpoints()
           .PublishAsAzureContainerApp((module, app) =>
           {
                 app.Tags.Add("domain", new BicepValue<string>("weather"));
                app.Template.Scale.MinReplicas = 1;
           });

//************CONSOLE APP************

if (builder.Environment.IsDevelopment() && builder.ExecutionContext.IsRunMode)
{
    var consoleAppCommand = Path.GetFullPath("../Faug.Demo.Weather.Migration/bin/Debug/net8.0/Faug.Demo.Weather.Migration.exe");
    var consoleAppWorkingDirectory = Path.GetFullPath("../Faug.Demo.Weather.Migration/bin/Debug/net8.0/");
    var consoleApp = builder.AddExecutable("tenantService", consoleAppCommand, consoleAppWorkingDirectory);
}


#endregion

#region Location

//************LOCATION API************

/*
var userAssignedIdentity = builder.AddBicepTemplate("user-assigned-identity", "infra/user-assigned-identity.bicep")
                            .WithParameter("name", "id-location-api");

var userAssignedIdentityId = userAssignedIdentity.GetOutput("id");
var userAssignedPrincipalId = userAssignedIdentity.GetOutput("principalId");
var userAssignedClientId = userAssignedIdentity.GetOutput("clientId");
*/

var redis = builder.AddRedis("location-cache");

var location = builder.AddProject<Projects.Faug_Demo_Location_Api>("location-api")
           .WithEnvironment("ASPNETCORE_ENVIRONMENT", environment)
           .WithReference(keycloak)
           .WaitFor(keycloak)
           .WithReference(redis)
           .WaitFor(redis)           
           .WithHttpsEndpoint(port: (builder.ExecutionContext.IsPublishMode || !builder.Environment.IsDevelopment() ? httpsPort : 2443)).WithExternalHttpEndpoints()
           .PublishAsAzureContainerApp((module, app) =>
           {
               app.Tags.Add("domain", new BicepValue<string>("location"));
               app.Template.Scale.MinReplicas = 1;
           });

#endregion

#region Frontend

//************FRONTEND************

var frontendClientSecret = builder.AddParameter("frontend-client-secret", secret: true);

var frontend = builder.AddProject<Projects.Faug_Demo_Frontend>("frontend")
       .WithEnvironment("ASPNETCORE_ENVIRONMENT", environment)
       .WaitFor(keycloak)
       .WithReference(keycloak)
       .WaitFor(weatherapi)
       .WithReference(weatherapi)
       .WaitFor(location)
       .WithReference(location)
       .WithEnvironment("frontend-client-secret", frontendClientSecret)
       .WithHttpsEndpoint(port: (builder.ExecutionContext.IsPublishMode || !builder.Environment.IsDevelopment() ? httpsPort : 3443)).WithExternalHttpEndpoints()
       .PublishAsAzureContainerApp((module, app) =>
        {
            app.Tags.Add("domain", new BicepValue<string>("frontend"));
            app.Template.Scale.MinReplicas = 1;
        });

#endregion

builder.Build().Run();