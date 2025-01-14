using Faug.Demo.ServiceDefaults;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//*******

//https://github.com/charlehsin/net6-react-tutorial

/*
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/build";
});
*/
//*******


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/_api/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

//*******
/*
app.UseStaticFiles();



app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "ClientApp", "build"))
});
app.UseSpaStaticFiles();*/

/*

//app.UseStaticFiles();    //Serve files from wwwroot
app.UseFileServer(new FileServerOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "ClientApp", "build"))
});
*/

app.UseSpa(spa =>
{
    if (builder.Environment.IsDevelopment())
    {
        // DEVELOPMENT
        // This gets the web root, and adds your path to your Angular folder.
        spa.Options.SourcePath = Path.Combine("ClientApp");

        // This calls the Angular CLI to build your Angular
        // project and call it via its server. This will
        // Load the Angular app into the browser, too,
        // And call your WebAPI for data.
        spa.UseReactDevelopmentServer(npmScript: "start");

    }
    else
    {
        // PRODUCTION
        // DO NOT CALL THE Angular CLI and dev server in-memory
        // when in Production as will call only the static files!
    }
});





/*
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "ClientApp";

    if (builder.Environment.IsDevelopment())
    {
        spa.UseReactDevelopmentServer(npmScript: "start");
    }
});*/

//*******

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
