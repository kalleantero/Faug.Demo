@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param frontend_containerport string

param outputs_azure_container_apps_environment_default_domain string

@secure()
param frontend_client_secret_value string

param outputs_azure_container_registry_managed_identity_id string

param outputs_managed_identity_client_id string

param outputs_azure_container_apps_environment_id string

param outputs_azure_container_registry_endpoint string

param frontend_containerimage string

resource frontend 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'frontend'
  location: location
  properties: {
    configuration: {
      secrets: [
        {
          name: 'frontend-client-secret'
          value: frontend_client_secret_value
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: frontend_containerport
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
          image: frontend_containerimage
          name: 'frontend'
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
              value: frontend_containerport
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
              name: 'services__weather-api__https__0'
              value: 'https://weather-api.${outputs_azure_container_apps_environment_default_domain}'
            }
            {
              name: 'services__location-api__https__0'
              value: 'https://location-api.${outputs_azure_container_apps_environment_default_domain}'
            }
            {
              name: 'frontend-client-secret'
              secretRef: 'frontend-client-secret'
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
    domain: 'frontend'
  }
}