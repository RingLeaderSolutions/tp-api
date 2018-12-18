param (
  [Parameter(Mandatory=$true)][string]$subscriptionId,
  [Parameter(Mandatory=$true)][string]$environmentName,
  [string]$kubernetesServicePrincipalId,
  [string]$region = "westeurope"
)

. 'common/environment-names.ps1'
$environment = parse-environment-name $environmentName

# Ensure we're using the correct subscription
az account set -s $subscriptionId

$resourceGroup = "theta-$environment"

# Set the default context to this environment's cluster
az aks get-credentials --resource-group $resourceGroup --name "$resourceGroup-cluster"
kubectl config use-context "$resourceGroup-cluster"

# Configure your Kubernetes cluster to run Azure AD Pod Identity infrastructure
kubectl create -f https://raw.githubusercontent.com/Azure/aad-pod-identity/master/deploy/infra/deployment.yaml

# Create an Azure managed identity
$servicesIdentity = "$resourceGroup-services-identity"
$identityResponseJson = az identity create -n $servicesIdentity -g $resourceGroup
$identityResponse = $identityResponseJson | ConvertFrom-Json
$princialId = $identityResponse.principalId
$clientId = $identityResponse.clientId
Write-Host "Created managed service identity with principal id '$princialId' and client id '$clientId'"

# Add permissions to get and list secrets
Write-Host "Waiting 60 seconds for Managed Service Identity resource to become available before adding permissions..."
Start-Sleep -Seconds 60
az role assignment create --role "Reader" --assignee $princialId --scope /subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.KeyVault/vaults/$resourceGroup-platform-vault
az keyvault set-policy -n $resourceGroup-platform-vault --secret-permissions get list --spn $clientId

# This is only required if the AKS Service Prinicipal was not automatically created with the cluster
# You can check if the SP has this role in the AKS 'Access Policy' section in the portal
#az role assignment create --role "Managed Identity Operator" --assignee $kubernetesServicePrincipalId --scope /subscriptions/$subscriptionId/resourceGroups/$resourceGroup/providers/Microsoft.ManagedIdentity/userAssignedIdentities/$servicesIdentity

# Token replace Kubernetes YAML files
$azureIdentityYaml = Get-Content "Azure-Identity-template.yaml"
$azureIdentityYaml = $azureIdentityYaml.replace('{subscriptionId}', $subscriptionId)
$azureIdentityYaml = $azureIdentityYaml.replace('{resourceGroup}', $resourceGroup)
$azureIdentityYaml = $azureIdentityYaml.replace('{identity}', $servicesIdentity)
$azureIdentityYaml = $azureIdentityYaml.replace('{clientId}', $clientId)
$azureIdentityYaml | Set-Content "Azure-Identity-$environment-$subscriptionId.yaml"

$azureIdentityBindingYaml = Get-Content "Azure-Identity-Binding-template.yaml"
$azureIdentityBindingYaml | Set-Content "Azure-Identity-Binding-$environment-$subscriptionId.yaml"

# Create Kubernetes AzureIdentity and AzureIdentityBinding
kubectl apply -f "Azure-Identity-$environment-$subscriptionId.yaml"
kubectl apply -f "Azure-Identity-Binding-$environment-$subscriptionId.yaml"

# Clean up
Remove-Item "Azure-Identity-$environment-$subscriptionId.yaml"
Remove-Item "Azure-Identity-Binding-$environment-$subscriptionId.yaml"