using Aspire.Hosting.Azure;
using Azure.Provisioning.Sql;
using Azure.Provisioning;

namespace Faug.Demo.AppHost.Extensions;

internal static class SqlResourceExtensions
{
    public static IResourceBuilder<SqlServerServerResource> AddLocalSqlServer(this IDistributedApplicationBuilder builder, IResourceBuilder<ParameterResource> serverName, IResourceBuilder<ParameterResource> adminPassword)
    {
        return builder.AddSqlServer(serverName.Resource.Value, password: adminPassword, port: 1450).WithLifetime(ContainerLifetime.Persistent);
    }

    public static IResourceBuilder<AzureSqlServerResource> AddCloudSqlServer(this IDistributedApplicationBuilder builder, IResourceBuilder<ParameterResource> serverName, IResourceBuilder<ParameterResource> adminPassword)
    {
        var sqlServer =  builder.AddAzureSqlServer(serverName.Resource.Value);
        sqlServer.ConfigureInfrastructure(infra =>
        {
            var resources = infra.GetProvisionableResources();
            var sqlServer = resources.OfType<SqlServer>().Single();
            sqlServer.PublicNetworkAccess = new BicepValue<ServerNetworkAccessFlag>("Enabled");
        });
        return sqlServer;
    }
}


