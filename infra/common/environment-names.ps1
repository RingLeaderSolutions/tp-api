## Enforce environment name convention with this enum
## All environments should have a 3 letter code to keep Azure resource names within maximum allowed lengths
## An environment name could be internal e.g. 'dev' or a client's production environment e.g. 'xxx'

enum EnvironmentNames
{
   dev
   uat
   prd
}

function parse-environment-name {
  param (
    [string]$environmentName
  )
  
  $ErrorActionPreference = "Stop"
  $environment = [EnvironmentNames]$environmentName
  Write-Host "Valid environment '$environment'"

  return $environment
}
