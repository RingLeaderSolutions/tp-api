param (
  [Parameter(Mandatory=$true)][string]$subscriptionId,
  [Parameter(Mandatory=$true)][string]$environmentName,
  [string]$region = "westeurope"
)

. 'common/environment-names.ps1'
$environment = parse-environment-name $environmentName

# Ensure we're using the correct subscription
az account set -s $subscriptionId

# Create a conventionally named resource group if it doesn't already exist (e.g. theta-dev)
$groupExists = az group exists -n theta-$environment
if ($groupExists -eq 'false')
{
  az group create -n theta-$environment -l $region
}

# Create a conventionally named key vault for the platform (e.g. theta-dev-platform)
az keyvault create -n theta-$environment-platform -g theta-$environment -l $region

# Add a dummy secret for testing purposes
az keyvault secret set -n testSecret --vault-name theta-$environment-platform --value MySuperSecretThatIDontWantToShareWithYou!
