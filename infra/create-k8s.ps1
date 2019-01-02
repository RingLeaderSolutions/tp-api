param (
  [Parameter(Mandatory=$true)][string]$subscriptionId,
  [Parameter(Mandatory=$true)][string]$environmentName,
  [Parameter(Mandatory=$true)][ValidateSet('staging','prod')][string]$letsencryptEnvironment,
  [Parameter(Mandatory=$true)][string]$acmeEmail,
  [string]$location = "westeurope"
)

. 'common/environment-names.ps1'
$environment = parse-environment-name $environmentName

# Ensure we're using the correct subscription
az account set -s $subscriptionId

$resourceGroup = "theta-$environment"

# TODO - create a global ACR resource

# If recreating the cluster, delete the old context
kubectl config delete-context "$resourceGroup-cluster"

# Setup of the AKS cluster (node size and count could be script parameters)
#Write-Host "Creating AKS cluster, this could take ~10 mins"
#az aks create -l $location -n "$resourceGroup-cluster" -g $resourceGroup --generate-ssh-keys -k 1.11.5 -c 1 -s Standard_B2s --disable-rbac

# Set the default context to this environment's cluster
az aks get-credentials --resource-group $resourceGroup --name "$resourceGroup-cluster" --overwrite-existing
kubectl config use-context "$resourceGroup-cluster"

# Setup tiller for Helm
kubectl create serviceaccount tiller --namespace kube-system
kubectl create clusterrolebinding tiller --clusterrole cluster-admin --serviceaccount=kube-system:tiller

# Setup the environment namespace
kubectl create namespace $environment
kubectl create clusterrolebinding default-view --clusterrole=view --serviceaccount=$environment:default

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
helm init --upgrade --wait --service-account default
helm install stable/nginx-ingress --namespace kube-system --set controller.service.loadBalancerIP="$publicIpAddress" --set controller.replicaCount=2 --set rbac.create=false

# Install cert-manager, which provides automatic Lets Encrypt certificate generation and management functionality
helm install stable/cert-manager --namespace kube-system --set ingressShim.defaultIssuerName=letsencrypt-$letsencryptEnvironment --set ingressShim.defaultIssuerKind=ClusterIssuer --set rbac.create=false --set serviceAccount.create=false

# Token replace Kubernetes YAML file
$clusterIssuerYaml = Get-Content "Cluster-Issuer-template.yaml"
$clusterIssuerYaml = $clusterIssuerYaml.replace('{letsencryptEnvironment}', $letsencryptEnvironment)
if ($letsencryptEnvironment -eq 'prod') 
{
  $clusterIssuerYaml = $clusterIssuerYaml.replace('{letsencryptServer}', 'https://acme-v02.api.letsencrypt.org/directory')
}
else 
{
  $clusterIssuerYaml = $clusterIssuerYaml.replace('{letsencryptServer}', 'https://acme-staging-v02.api.letsencrypt.org/directory')
}
$clusterIssuerYaml = $clusterIssuerYaml.replace('{acmeEmail}', $acmeEmail)
$clusterIssuerYaml | Set-Content "Cluster-Issuer-$environment-$subscriptionId.yaml"

# Create a CA cluster issuer
kubectl apply -f "Cluster-Issuer-$environment-$subscriptionId.yaml"

# Token replace Kubernetes YAML file
$certificateYaml = Get-Content "Certificate-template.yaml"
$certificateYaml = $certificateYaml.replace('{letsencryptEnvironment}', $letsencryptEnvironment)
$certificateYaml = $certificateYaml.replace('{environment}', $environment)
$certificateYaml = $certificateYaml.replace('{location}', $location)
$certificateYaml | Set-Content "Certificate-$environment-$subscriptionId.yaml"

# Create a certificate object
kubectl apply -f "Certificate-$environment-$subscriptionId.yaml"

# Clean up
Remove-Item "Cluster-Issuer-$environment-$subscriptionId.yaml"
Remove-Item "Certificate-$environment-$subscriptionId.yaml"