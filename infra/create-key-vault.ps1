param (
  [Parameter(Mandatory=$true)][string]$subscriptionId,
  [Parameter(Mandatory=$true)][string]$environmentName,
  [string]$location = "westeurope"
)

. 'common/environment-names.ps1'
$environment = parse-environment-name $environmentName

# Ensure we're using the correct subscription
az account set -s $subscriptionId

$resourceGroup = "theta-$environment"

# Create a conventionally named resource group if it doesn't already exist (e.g. theta-dev)
$groupExists = az group exists -n $resourceGroup
if ($groupExists -eq 'false')
{
  az group create -n $resourceGroup -l $location
}

# Create a conventionally named key vault for the platform (e.g. theta-dev-platform)
az keyvault create -n $resourceGroup-platform-vault -g $resourceGroup -l $location

# Add a dummy secret for testing purposes
az keyvault secret set -n testSecret --vault-name $resourceGroup-platform-vault --value MySuperSecretThatIDontWantToShareWithYou!
