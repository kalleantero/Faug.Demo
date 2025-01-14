using System.Net;

/*
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
*/

using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.FileProviders;

/*
using MyBusiness.Bff.Authentication.Extensions;
using MyBusiness.Bff.Endpoints.Extensions;
using MyBusiness.Bff.Extensions;
using MyBusiness.Bff.Interfaces;
using MyBusiness.Bff.Localization;
using MyBusiness.Bff.Logging;
using MyBusiness.Bff.Middlewares;
using MyBusiness.Bff.Proxy.Extensions;
using MyBusiness.Bff.Security.Extensions;
using MyBusiness.Bff.Services;
using MyBusiness.Bff.Shared;
using MyBusiness.ServerSideSessions;
*/

/*
 * 
 * using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("MyB Frontend with BFF is starting up.");

*/

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddHttpContextAccessor();

    var env = builder.Configuration["ASPNETCORE_ENVIRONMENT"];

    /*
    builder.Host.UseSerilog((ctx, lc) => lc
        .Enrich.FromLogContext()
        .Enrich.WithAssemblyName()
        .Enrich.WithProperty("Environment", env)
        .Enrich.WithProperty("ApplicationName", "MyBusiness.Frontend.Bff")
        .ConfigureLogLevels(env)
        .AddLoggingSinks(ctx.Configuration));
    */

    /*
    var keyVaultUri = builder.Configuration["Authentication:KeyVaultUrl"];
    ArgumentNullException.ThrowIfNull(keyVaultUri, nameof(keyVaultUri));
    var secretName = builder.Configuration["Logging:ApplicationInsights:ConnectionStringKeyVaultSecretName"];

    if (!string.IsNullOrEmpty(secretName))
    {
        var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

        // The following line enables Application Insights telemetry collection.
        builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions()
        {
            ConnectionString = secretClient.GetSecret(secretName).Value.Value
        });
    }*/

    builder.Services.AddHttpLogging(o => o = new HttpLoggingOptions());

    /*
    // Add reverse proxy configuration (YARP)
    builder.Services.AddReverseProxy(builder.Configuration);

    // Add authentication configuration
    builder.Services.AddAuthentication(builder.Configuration);
    */

    //builder.Services.AddFeatureFlags();

    // Add deeplinking configuration

    //builder.Services.AddDeeplinking(builder.Configuration);
    // HTTP Client is used to fetch Web Component javascript content
    builder.Services.AddHttpClient();
    
    /*
    //Register IUserService for fetching user info from Users API
    builder.Services.AddHttpClient<IUserApiService, UserApiService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ReverseProxyRulesForBackend:Clusters:usersApi:Destinations:usersApi/destination1:Address"));
    });

    builder.Services.AddHttpClient<ICompanyApi, CompanyApi>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ReverseProxyRulesForBackend:Clusters:companyApi:Destinations:companyApi/destination1:Address"));
    });

    builder.Services.AddHttpClient<IEventSenderApiService, EventSenderApiService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ReverseProxyRulesForBackend:Clusters:eventsApi:Destinations:eventsApi/destination1:Address"));
    });

    builder.Services.AddHttpClient<IIfHelpChatService, IfHelpChatService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ReverseProxyRulesForWebComponents:Clusters:ifHelpWebComponentApi:Destinations:ifHelpWebComponentApi/destination:Address"));
    });

    builder.Services.AddHttpClient<IFeedbackService, FeedbackFunctionService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("FEEDBACK_FUNCTIONS_API_URI"));
        client.DefaultRequestHeaders.Add("x-functions-key", builder.Configuration.GetValue<string>("FEEDBACK_FUNCTIONS_API_KEY"));
    });

    builder.Services.AddServerSideSessions(builder.Configuration);

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IEventSenderService, EventSenderService>();


    if (builder.Configuration["Localization:FileProvider"] == "Crowdin")
    {
        builder.Services.AddCrowdinLocalizationServices();
    }
    else
    {
        builder.Services.AddLocalizationServices();
    }

    */

    // Add authorization configuration
    builder.Services.AddAuthorization(options =>
    {
        // This is a default authorization policy which requires authentication
        options.AddPolicy("RequireAuthenticatedUserPolicy", policy =>
        {
            policy.RequireAuthenticatedUser();
        });
    });


    // https://docs.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-6.0
    // NODE.JS: app.use(require('compression')({ level: 9 }));
    builder.Services.AddResponseCompression(options =>
    {
        //content-encoding: gzip
        options.Providers.Add<GzipCompressionProvider>();
    });

    /*
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardLimit = 2;
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
        //Because Azure App service is behind one proxy, which seem to change on every installation, its impossible to get this to work with KnownProxies. 
        var allowedProxies = builder.Configuration.GetConfigurationValuesAsList("AllowedProxy");

        foreach (var allowedProxy in allowedProxies)
        {
            options.KnownProxies.Add(IPAddress.Parse(allowedProxy));
        }
    });
    */

    builder.Services.AddControllers();

    builder.Services.AddSpaStaticFiles(configuration =>
    {
        configuration.RootPath = "dist";
    });

    var app = builder.Build();
   // var logger = app.Services.GetRequiredService<Serilog.ILogger>();
    //Run migrations from Session DB
    //app.RunDatabaseMigrations();

    //For F5 Proxy
   // app.UseMiddleware<ClientIpMiddleware>();
    app.UseForwardedHeaders();

    app.UseHttpLogging();
    app.UseResponseCompression();
    // Enables secure header configuration provider by NetEscapades.AspNetCore.SecurityHeaders library
    //app.UseSecureHeaders(builder.Configuration, app.Environment.IsDevelopment());

    //app.UseFontCachingMiddleware();

    app.UseCors("Frontend");

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        // Strict-Transport-Security (HSTS)
        // It tells the browser: "You shall only access this URL over a secure connection.".
        // By submitting a Strict-Transport-Security header, the browser saves it and redirects itself to the HTTPS version without making an insecure call.
        // Source: https://hamedfathi.me/a-professional-asp.net-core-api-security-headers/
        app.UseHsts();
    }

    if (bool.TryParse(builder.Configuration["UseHttpsRedirect"], out bool useHttpsRedirect) && useHttpsRedirect)
    {
        app.UseHttpsRedirection();
    }
    // Serve files from wwwroot
    app.UseStaticFiles();


    // In production, serve React files and default page from wwwroot/assets
    //if (!app.Environment.IsDevelopment())
    // {
    /*
    app.UseFileServer(new FileServerOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "wwwroot", "assets"))
    });*/

    //     app.UseFileServer(new FileServerOptions
    //     {
    //         FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "wwwroot", "assets"))
    //     });
    // }



    app.UseSpa(spa =>
    {
        // To learn more about options for serving an Angular SPA from ASP.NET Core,
        // see https://go.microsoft.com/fwlink/?linkid=864501

        spa.Options.SourcePath = "src";

       // if (env.IsDevelopment())
       // {
            spa.UseReactDevelopmentServer(npmScript: "start");
       // }
    });

    app.UseRouting();

    // Enable authentication middleware in pipeline
    app.UseAuthentication();
    
    // Enable routing middleware in pipeline
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        /*
        // Enable YARP reverse proxy middleware in pipeline
        endpoints.MapReverseProxy();
        // Enable login and logout endpoints
        endpoints.MapLoginEndpoints(logger);
        // Enable deeplinks
        endpoints.MapDeeplinkEndpoints(builder.Configuration, logger, app.Environment.IsDevelopment());
        // Enable settings (environment variable) endpoints
        endpoints.MapSettingsEndpoints(builder.Configuration, logger);
        // Enable redirection endpoints
        endpoints.MapRedirectionEndpoints(builder.Configuration, logger, app.Environment.IsDevelopment());
        // Enable web component script loading endpoints
        endpoints.MapWebComponentEndpoints(builder.Configuration, logger);
        // Enable additional endpoints
        endpoints.MapOtherEndpoints(builder.Configuration, logger);

        // Enable feedback endpoints
        endpoints.MapFeedbackEndpoints(builder.Configuration, logger);

        endpoints.MapLocalizationEndpoints(logger);
        */

        endpoints.MapControllers();

    });

    app.MapFallbackToFile("assets/index.html");

    app.Run();

}
catch (Exception ex)
{
   // Log.Fatal(ex, "Unhandled exception occured.");
}
finally
{
  //  Log.Information("MyB Frontend with BFF shut down complete.");
   // Log.CloseAndFlush();
}
