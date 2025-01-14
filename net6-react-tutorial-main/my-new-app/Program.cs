using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using my_new_app;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();
