using Faug.Demo.AppHost.Util;
using Microsoft.Extensions.Hosting;

namespace Faug.Demo.AppHost.Util;

internal static class ResourceExtensions
{

    public static IResourceBuilder<SqlServerServerResource> AddSqlServer(this IDistributedApplicationBuilder builder)
    {
        IResourceBuilder<SqlServerServerResource> sqlServer;
        if (builder.Environment.IsDevelopment() && !builder.ExecutionContext.IsPublishMode)
        {
            var sqlPassword = builder.AddParameter("password", secret: true);
            sqlServer = builder.AddSqlServer("sql", password: sqlPassword);
        }
        else
        {
            sqlServer = builder.AddSqlServer("sql-mybusiness").PublishAsAzureSqlDatabase();
        }

        return sqlServer;
    }

    public static IResourceBuilder<IResourceWithConnectionString> AddApplicationInsights(this IDistributedApplicationBuilder builder)
    {
        if (builder.ExecutionContext.IsPublishMode)
        {
            var laws = builder.AddAzureLogAnalyticsWorkspace("laws-mybusiness");
            var insights = builder.AddAzureApplicationInsights("mybusiness-appinsights", laws);
            return insights;
        }
        else
        {
            return builder.AddConnectionString("mybusiness-appinsights", "APPLICATIONINSIGHTS_CONNECTION_STRING");
        }
    }

    public static IResourceBuilder<SqlServerDatabaseResource> AddUsersDatabase(this IResourceBuilder<SqlServerServerResource> sqlServer)
    {
        return sqlServer.AddDatabase("sqldb-mybusiness-users");
    }

    public static IResourceBuilder<SqlServerDatabaseResource> AddCompanyDatabase(this IResourceBuilder<SqlServerServerResource> sqlServer)
    {
        return sqlServer.AddDatabase("sqldb-mybusiness-company");
    }

    public static IResourceBuilder<IResourceWithConnectionString> AddKeyvault(this IDistributedApplicationBuilder builder)
    {

        var keyVault = builder.ExecutionContext.IsPublishMode
            ? builder.AddAzureKeyVault("kv-mybusiness")
            : builder.AddConnectionString("kv-mybusiness");

        return keyVault;
    }

    public static IResourceBuilder<IResourceWithConnectionString> AddServiceBus(this IDistributedApplicationBuilder builder)
    {

        var serviceBus = builder.ExecutionContext.IsPublishMode
        ? builder.AddAzureServiceBus("sb-mybusiness")
        : builder.AddConnectionString("sb-mybusiness");

        return serviceBus;
    }

    public static IResourceBuilder<IResourceWithConnectionString> AddDistributedCache(this IDistributedApplicationBuilder builder, string cacheName)
    {
        var distributedCache = builder.AddRedis(cacheName);
        //.WithDataVolume().WithPersistence(interval: TimeSpan.FromMinutes(10), keysChangedThreshold: 100);
        return distributedCache;
    }


}
