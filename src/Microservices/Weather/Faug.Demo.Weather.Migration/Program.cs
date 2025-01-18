// See https://aka.ms/new-console-template for more information
using Faug.Demo.ServiceDefaults;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Json;


Console.WriteLine("Hello, World!");

/*
var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHttpClient();

    // Grab a new client from the service provider
    var client = provider.GetService<HttpClient>()!;



builder.Services.AddHttpClient<WeatherClient>(
    static client => client.BaseAddress = new("https://weatherapi"));



//builder.AddServiceDefaults();
//builder.Services.AddHostedService<Worker>();
//builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddSqlServerDbContext<CompanyContext>(DatabaseNames.CompanyDatabase);
var host = builder.Build();




host.Run();


public class WeatherClient
{
    private readonly HttpClient _httpClient;

    public WeatherClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<object> GetRandomJokeAsync() => await _httpClient.GetFromJsonAsync<object>("jokes/random");
}

*/