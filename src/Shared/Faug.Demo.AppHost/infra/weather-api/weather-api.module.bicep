@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param weather_api_containerport string

param outputs_azure_container_apps_environment_default_domain string

@secure()
param sql_password_value string

param outputs_azure_container_registry_managed_identity_id string

param outputs_managed_identity_client_id string

param outputs_azure_container_apps_environment_id string

param outputs_azure_container_registry_endpoint string

param weather_api_containerimage string

resource acalocationapi 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'weather-api'
  location: location
  properties: {
    configuration: {
      secrets: [
        {
          name: 'connectionstrings--weather-db'
          value: '${'Server=weather-db-server,1433;User ID=sa;Password=${sql_password_value};TrustServerCertificate=true'};Database=weather-db'
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: weather_api_containerport
        transport: 'http'
      }
      registries: [
        {
          server: outputs_azure_container_registry_endpoint
          identity: outputs_azure_container_registry_managed_identity_id
        }
      ]
    }
    environmentId: outputs_azure_container_apps_environment_id
    template: {
      containers: [
        {
          image: weather_api_containerimage
          name: 'weather-api'
          env: [
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES'
              value: 'true'
            }
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES'
              value: 'true'
            }
            {
              name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY'
              value: 'in_memory'
            }
            {
              name: 'ASPNETCORE_FORWARDEDHEADERS_ENABLED'
              value: 'true'
            }
            {
              name: 'HTTP_PORTS'
              value: weather_api_containerport
            }
            {
              name: 'HTTPS_PORTS'
              value: weather_api_containerport
            }
            {
              name: 'services__keycloak-idp__http__0'
              value: 'http://keycloak-idp.${outputs_azure_container_apps_environment_default_domain}'
            }
            {
              name: 'ConnectionStrings__weather-db'
              secretRef: 'connectionstrings--weather-db'
            }
            {
              name: 'AZURE_CLIENT_ID'
              value: outputs_managed_identity_client_id
            }
          ]
        }
      ]
      scale: {
        minReplicas: 4
      }
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${outputs_azure_container_registry_managed_identity_id}': { }
    }
  }
  tags: {
    custom_tag: 'dw'
  }
}