@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param principalId string

param principalName string

resource weather_db_server 'Microsoft.Sql/servers@2021-11-01' = {
  name: take('weatherdbserver-${uniqueString(resourceGroup().id)}', 63)
  location: location
  properties: {
    administrators: {
      administratorType: 'ActiveDirectory'
      login: principalName
      sid: principalId
      tenantId: subscription().tenantId
      azureADOnlyAuthentication: true
    }
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    version: '12.0'
  }
  tags: {
    'aspire-resource-name': 'weather-db-server'
  }
}

resource sqlFirewallRule_AllowAllAzureIps 'Microsoft.Sql/servers/firewallRules@2021-11-01' = {
  name: 'AllowAllAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
  parent: weather_db_server
}

resource weather_db 'Microsoft.Sql/servers/databases@2021-11-01' = {
  name: 'weather-db'
  location: location
  parent: weather_db_server
}

output sqlServerFqdn string = weather_db_server.properties.fullyQualifiedDomainName