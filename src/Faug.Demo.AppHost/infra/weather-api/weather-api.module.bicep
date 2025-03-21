@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param weather_api_containerport string

param outputs_azure_container_apps_environment_default_domain string

param weather_db_server_outputs_sqlserverfqdn string

param outputs_azure_container_registry_managed_identity_id string

param outputs_managed_identity_client_id string

param outputs_azure_container_apps_environment_id string

param outputs_azure_container_registry_endpoint string

param weather_api_containerimage string

resource weather_api 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'weather-api'
  location: location
  properties: {
    configuration: {
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
              name: 'HTTPS_PORTS'
              value: weather_api_containerport
            }
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Development'
            }
            {
              name: 'services__keycloak-idp__http__0'
              value: 'http://keycloak-idp.${outputs_azure_container_apps_environment_default_domain}'
            }
            {
              name: 'ConnectionStrings__weather-db'
              value: '${'Server=tcp:${weather_db_server_outputs_sqlserverfqdn},1433;Encrypt=True;Authentication="Active Directory Default"'};Database=weather-db'
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
  tags: {
    domain: 'weather'
  }
}