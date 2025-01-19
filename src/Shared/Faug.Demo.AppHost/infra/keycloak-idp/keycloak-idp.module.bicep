@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param keycloak_idp_volumes_0_storage string

param keycloak_username_value string

@secure()
param keycloak_password_value string

param outputs_azure_container_registry_managed_identity_id string

param outputs_managed_identity_client_id string

param outputs_azure_container_apps_environment_id string

resource keycloak_idp 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'keycloak-idp'
  location: location
  properties: {
    configuration: {
      secrets: [
        {
          name: 'keycloak-admin-password'
          value: keycloak_password_value
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 8080
        transport: 'http'
      }
    }
    environmentId: outputs_azure_container_apps_environment_id
    template: {
      containers: [
        {
          image: 'quay.io/keycloak/keycloak:26.0'
          name: 'keycloak-idp'
          args: [
            'start'
            '--import-realm'
          ]
          env: [
            {
              name: 'KEYCLOAK_ADMIN'
              value: keycloak_username_value
            }
            {
              name: 'KEYCLOAK_ADMIN_PASSWORD'
              secretRef: 'keycloak-admin-password'
            }
            {
              name: 'AZURE_CLIENT_ID'
              value: outputs_managed_identity_client_id
            }
          ]
          volumeMounts: [
            {
              volumeName: 'v0'
              mountPath: '/opt/keycloak/data'
            }
          ]
        }
      ]
      scale: {
        minReplicas: 1
      }
      volumes: [
        {
          name: 'v0'
          storageType: 'AzureFile'
          storageName: keycloak_idp_volumes_0_storage
        }
      ]
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${outputs_azure_container_registry_managed_identity_id}': { }
    }
  }
}