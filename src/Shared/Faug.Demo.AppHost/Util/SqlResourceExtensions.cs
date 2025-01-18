using Faug.Demo.AppHost.Util;
using Microsoft.Extensions.Hosting;

namespace Faug.Demo.AppHost.Util;

/*
internal static class SqlResourceExtensions
{
    public static IResourceBuilder<SqlServerServerResource> AddSqlServer(this IDistributedApplicationBuilder builder, ParameterResource resourceName, ParameterResource password = null)
    {
        IResourceBuilder<SqlServerServerResource> sqlServer;
        if (builder.Environment.IsDevelopment() && !builder.ExecutionContext.IsPublishMode && password != null)
        {
            sqlServer = builder.AddSqlServer(resourceName, password);
        }
        else
        {
            sqlServer = builder.AddSqlServer(resourceName).PublishAsAzureSqlDatabase();
        }

        return sqlServer;
    }

    public static IResourceBuilder<SqlServerDatabaseResource> AddDatabase(this IResourceBuilder<SqlServerServerResource> sqlServer, string databaseName)
    {
        return sqlServer.AddDatabase(databaseName);
    }
}
*/
internal static class ApplicationAnalyticsResourceExtensions
{
    public static IResourceBuilder<IResourceWithConnectionString> AddApplicationInsights(this IDistributedApplicationBuilder builder, string resourceName)
    {
        if (builder.ExecutionContext.IsPublishMode)
        {
            var laws = builder.AddAzureLogAnalyticsWorkspace("laws");
            var insights = builder.AddAzureApplicationInsights("appinsights", laws);
            return insights;
        }
        else
        {
            return builder.AddConnectionString("appinsights", "APPLICATIONINSIGHTS_CONNECTION_STRING");
        }
    }
}

internal static class KeyVaultResourceExtensions
{
    public static IResourceBuilder<IResourceWithConnectionString> AddKeyvault(this IDistributedApplicationBuilder builder, string resourceName)
    {
        var fullResourceName = "kv-" + resourceName;

        var keyVault = builder.ExecutionContext.IsPublishMode
            ? builder.AddAzureKeyVault(fullResourceName)
            : builder.AddConnectionString(fullResourceName);

        return keyVault;
    }
}

internal static class ServiceBusResourceExtensions
{
    public static IResourceBuilder<IResourceWithConnectionString> AddServiceBus(this IDistributedApplicationBuilder builder, string resourceName)
    {
        var serviceBus = builder.ExecutionContext.IsPublishMode
        ? builder.AddAzureServiceBus(resourceName)
        : builder.AddConnectionString(resourceName);

        return serviceBus;
    }
}

