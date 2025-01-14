using Aspire.Hosting;
using Faug.Demo.AppHost.Util;

var builder = DistributedApplication.CreateBuilder(args);

//builder.AddProject<Projects.>

var sqlServer = builder.AddSqlServer();


var weatherapi = builder.AddProject<Projects.Faug_Demo_Company_Api>("weatherapi")
        .WithExternalHttpEndpoints();


//builder.AddProject<Projects.Faug_Demo_Frontend>("frontend")
//    .WithReference(weatherapi);

/*
builder.AddNpmApp("react", "../AspireJavaScript.React")
    .WithReference(weatherapi)
    .WaitFor(weatherapi)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();
*/

//builder.AddProject<Projects.FunctionApp1>("functionapp1");

builder.AddProject<Projects.Faug_Demo_Frontend>("frontendnew");


//builder.AddProject<Projects.Faug_Demo_Frontend2>("faug-demo-frontend2");


builder.Build().Run();
