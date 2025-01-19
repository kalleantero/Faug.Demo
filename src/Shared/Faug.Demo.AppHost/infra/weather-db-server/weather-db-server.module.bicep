@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

@secure()
param sql_password_value string

param outputs_azure_container_registry_managed_identity_id string

param outputs_managed_identity_client_id string

param outputs_azure_container_apps_environment_id string

resource weather_db_server 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'weather-db-server'
  location: location
  properties: {
    configuration: {
      secrets: [
        {
          name: 'mssql-sa-password'
          value: sql_password_value
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: false
        targetPort: 1433
        transport: 'tcp'
      }
    }
    environmentId: outputs_azure_container_apps_environment_id
    template: {
      containers: [
        {
          image: 'mcr.microsoft.com/mssql/server:2022-latest'
          name: 'weather-db-server'
          env: [
            {
              name: 'ACCEPT_EULA'
              value: 'Y'
            }
            {
              name: 'MSSQL_SA_PASSWORD'
              secretRef: 'mssql-sa-password'
            }
            {
              name: 'AZURE_CLIENT_ID'
              value: outputs_managed_identity_client_id
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
      }
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${outputs_azure_container_registry_managed_identity_id}': { }
    }
  }
}