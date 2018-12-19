param (
  [Parameter(Mandatory=$true)][string]$subscriptionId,
  [Parameter(Mandatory=$true)][string]$environmentName,
  [string]$region = "westeurope"
)

. 'common/environment-names.ps1'
$environment = parse-environment-name $environmentName

# Ensure we're using the correct subscription
az account set -s $subscriptionId

$resourceGroup = "theta-$environment"

# TODO - create AKS and ACR resources

# Set the default context to this environment's cluster
az aks get-credentials --resource-group $resourceGroup --name "$resourceGroup-cluster"
kubectl config use-context "$resourceGroup-cluster"

# Get AKS Resource Group, normally named starting MC_
$aksResourceGroup = az aks show --resource-group $resourceGroup --name "$resourceGroup-cluster" --query nodeResourceGroup -o tsv
Write-Host "Found AKS resource group '$aksResourceGroup'"

# Create a conventionally named static public ip address
$publicIpResponseJson = az network public-ip create --resource-group $aksResourceGroup --name "$resourceGroup-cluster-publicip" --allocation-method static
$publicIpResponse = $publicIpResponseJson | ConvertFrom-Json
$publicIpAddress = $publicIpResponse.publicIp.ipAddress
$publicIpId = $publicIpResponse.publicIp.id
Write-Host "Created static public ip with id '$publicIpId' and address '$publicIpAddress'"

# Set the DNS name
az network public-ip update --ids $publicIpId --dns-name "$resourceGroup"

# Deploy the nginx-ingress chart with Helm
helm init --client-only
helm install stable/nginx-ingress --namespace kube-system --set controller.service.loadBalancerIP="$publicIpAddress" --set controller.replicaCount=2 --set rbac.create=false

# TODO - enable TLS for HTTPS
