using './keycloak-idp.module.bicep'

param keycloak_password_value = '{{ securedParameter "keycloak_password" }}'
param keycloak_username_value = '{{ parameter "keycloak_username" }}'
param outputs_azure_container_apps_environment_id = '{{ .Env.AZURE_CONTAINER_APPS_ENVIRONMENT_ID }}'
param outputs_azure_container_registry_managed_identity_id = '{{ .Env.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID }}'
param outputs_managed_identity_client_id = '{{ .Env.MANAGED_IDENTITY_CLIENT_ID }}'
